namespace EventosBackend.Services.Interfaces
{
    public interface IPasswordHasher
    {
        (byte[] hash, byte[] salt) CreateHash(string password);
        bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
    }
}