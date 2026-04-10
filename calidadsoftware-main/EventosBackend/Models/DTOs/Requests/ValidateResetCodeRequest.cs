using System.ComponentModel.DataAnnotations;

public class ValidateResetCodeRequest
{
  [Required]
  [EmailAddress]
  public string Email { get; set; }

  [Required]
  public string Code { get; set; }
}