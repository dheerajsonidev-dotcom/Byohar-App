namespace Byohar.Application.Validators.Identity
{
    public class ActivateDeactivateRequest
    {
        public List<Guid> UserIds { get; set; }
        public bool Activate { get; set; }

    }
}
