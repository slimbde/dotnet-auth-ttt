namespace ttt.Models
{
  public class User
  {
    public int Id { get; set; }
    public string DomainName { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public string Email { get; set; }
    public string FIO { get; set; }
  }
}