

namespace StarSarcasm.Application.Interfaces.ISMSService
{
    public interface IOTPService
    {
        public Task<string> GenerateOTP(string email);
       
    }
}
