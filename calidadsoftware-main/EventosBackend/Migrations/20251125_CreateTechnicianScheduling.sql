-- =====================================================
-- Script SQL: Sistema de Gestión de Citas para Técnicos
-- Base de datos: Oracle
-- Fecha: 2025-11-25
-- =====================================================

-- Opcional: fijar el esquema actual si las tablas USUARIO existen en otro usuario
-- ALTER SESSION SET CURRENT_SCHEMA = <SU_ESQUEMA>;

-- Bloque de limpieza: elimina objetos si existen para re-ejecutar el script sin errores
DECLARE
    PROCEDURE drop_if_exists(p_sql IN VARCHAR2) IS
    BEGIN
        BEGIN
            EXECUTE IMMEDIATE p_sql;
        EXCEPTION
            WHEN OTHERS THEN
                IF SQLCODE IN (-942, -4080, -4043) THEN NULL; ELSE RAISE; END IF;
        END;
    END;
BEGIN
    drop_if_exists('DROP VIEW VW_HORARIOS_DISPONIBLES');
    drop_if_exists('DROP TRIGGER TRG_CITA_ACTUALIZAR');
    drop_if_exists('DROP PROCEDURE SP_GENERAR_HORARIOS_SEMANA');
    drop_if_exists('DROP TABLE CITA CASCADE CONSTRAINTS');
    drop_if_exists('DROP TABLE TECNICO_HORARIO CASCADE CONSTRAINTS');
END;
/

-- 1. Tabla TECNICO_HORARIO
-- Almacena bloques de tiempo disponibles/no disponibles por técnico
CREATE TABLE TECNICO_HORARIO (
    ID_HORARIO NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    ID_USUARIO VARCHAR2(50) NOT NULL,
    FECHA DATE NOT NULL,
    HORA_INICIO VARCHAR2(5) NOT NULL, -- Formato HH24:MI (08:00, 09:00, etc.)
    HORA_FIN VARCHAR2(5) NOT NULL,
    DISPONIBLE CHAR(1) DEFAULT '1' CHECK (DISPONIBLE IN ('0','1')),
    MOTIVO_BLOQUEO VARCHAR2(200),
    FECHA_CREACION TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_TECNICO_HORARIO_USUARIO FOREIGN KEY (ID_USUARIO) REFERENCES USUARIO(ID_UNICO) ON DELETE CASCADE
);

-- Índices para búsquedas frecuentes
CREATE INDEX IDX_TECNICO_HORARIO_USUARIO_FECHA ON TECNICO_HORARIO(ID_USUARIO, FECHA);
CREATE INDEX IDX_TECNICO_HORARIO_DISPONIBLE ON TECNICO_HORARIO(DISPONIBLE);

-- 2. Tabla CITA
-- Registro de citas agendadas (1 hora cada una)
CREATE TABLE CITA (
    ID_CITA NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    ID_USUARIO_CLIENTE VARCHAR2(50) NOT NULL,
    ID_USUARIO_TECNICO VARCHAR2(50) NOT NULL,
    FECHA_CITA DATE NOT NULL,
    HORA_INICIO VARCHAR2(5) NOT NULL,
    HORA_FIN VARCHAR2(5) NOT NULL,
    DESCRIPCION_PROBLEMA VARCHAR2(500),
    ESTADO VARCHAR2(20) DEFAULT 'PENDIENTE' CHECK (ESTADO IN ('PENDIENTE','CONFIRMADA','COMPLETADA','CANCELADA')),
    DIRECCION VARCHAR2(300),
    TELEFONO_CONTACTO VARCHAR2(20),
    FECHA_CREACION TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FECHA_ACTUALIZACION TIMESTAMP,
    CONSTRAINT FK_CITA_CLIENTE FOREIGN KEY (ID_USUARIO_CLIENTE) REFERENCES USUARIO(ID_UNICO) ON DELETE CASCADE,
    CONSTRAINT FK_CITA_TECNICO FOREIGN KEY (ID_USUARIO_TECNICO) REFERENCES USUARIO(ID_UNICO) ON DELETE CASCADE
);

-- Índices
CREATE INDEX IDX_CITA_TECNICO_FECHA ON CITA(ID_USUARIO_TECNICO, FECHA_CITA);
CREATE INDEX IDX_CITA_CLIENTE ON CITA(ID_USUARIO_CLIENTE);
CREATE INDEX IDX_CITA_ESTADO ON CITA(ESTADO);

-- 3. Trigger para actualizar FECHA_ACTUALIZACION automáticamente
CREATE OR REPLACE TRIGGER TRG_CITA_ACTUALIZAR
BEFORE UPDATE ON CITA
FOR EACH ROW
BEGIN
    :NEW.FECHA_ACTUALIZACION := CURRENT_TIMESTAMP;
END;
/

-- 4. Vista: Horarios disponibles de técnicos (excluye horas con citas)
CREATE OR REPLACE VIEW VW_HORARIOS_DISPONIBLES AS
SELECT 
    TH.ID_HORARIO,
    TH.ID_USUARIO,
    U.NOMBRE || ' ' || U.APELLIDO1 AS NOMBRE_TECNICO,
    U.EMAIL,
    TH.FECHA,
    TH.HORA_INICIO,
    TH.HORA_FIN,
    TH.DISPONIBLE,
    TH.MOTIVO_BLOQUEO,
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM CITA C 
            WHERE C.ID_USUARIO_TECNICO = TH.ID_USUARIO 
              AND C.FECHA_CITA = TH.FECHA 
              AND C.HORA_INICIO = TH.HORA_INICIO
              AND C.ESTADO IN ('PENDIENTE','CONFIRMADA')
        ) THEN '0'
        ELSE TH.DISPONIBLE
    END AS DISPONIBLE_REAL
FROM TECNICO_HORARIO TH
INNER JOIN USUARIO U ON TH.ID_USUARIO = U.ID_UNICO
WHERE U.TIPO_USUARIO = 'TECNICO';

-- 5. Procedimiento: Generar horarios semanales automáticamente
CREATE OR REPLACE PROCEDURE SP_GENERAR_HORARIOS_SEMANA(
    p_id_tecnico IN VARCHAR2,
    p_fecha_inicio IN DATE
) AS
    v_fecha DATE;
    v_dia_semana NUMBER;
    v_hora_slot VARCHAR2(5);
BEGIN
    -- Generar slots de L-V para una semana
    FOR dia IN 0..6 LOOP
        v_fecha := p_fecha_inicio + dia;
        -- Usar abreviatura de día en inglés para evitar dependencia NLS
        -- MON,TUE,WED,THU,FRI serán válidos
        -- Si su entorno usa otra configuración, puede mantener 'D' con territorio
        -- v_dia_semana := TO_NUMBER(TO_CHAR(v_fecha, 'D'));
        
        -- Solo L-V (2 al 6)
        IF TO_CHAR(v_fecha, 'DY', 'NLS_DATE_LANGUAGE=ENGLISH') IN ('MON','TUE','WED','THU','FRI') THEN
            -- Slots de 8-12 y 13-17 (excluyendo 12-13 almuerzo)
            FOR hora IN 8..16 LOOP
                IF hora != 12 THEN
                    v_hora_slot := LPAD(hora, 2, '0') || ':00';
                    
                    -- Insertar solo si no existe ya
                    INSERT INTO TECNICO_HORARIO (ID_USUARIO, FECHA, HORA_INICIO, HORA_FIN, DISPONIBLE)
                    SELECT p_id_tecnico, v_fecha, v_hora_slot, LPAD(hora+1, 2, '0') || ':00', '1'
                    FROM DUAL
                    WHERE NOT EXISTS (
                        SELECT 1 FROM TECNICO_HORARIO 
                        WHERE ID_USUARIO = p_id_tecnico 
                          AND FECHA = v_fecha 
                          AND HORA_INICIO = v_hora_slot
                    );
                END IF;
            END LOOP;
        END IF;
    END LOOP;
    COMMIT;
END;
/

-- 6. Ejemplo de uso: generar horarios para un técnico (ID 5) desde hoy
-- EXEC SP_GENERAR_HORARIOS_SEMANA(5, TRUNC(SYSDATE));

-- 7. Comentarios en tablas
COMMENT ON TABLE TECNICO_HORARIO IS 'Bloques horarios de técnicos (slots de 1h)';
COMMENT ON TABLE CITA IS 'Citas agendadas con técnicos';

-- =====================================================
-- FIN DEL SCRIPT
-- =====================================================
