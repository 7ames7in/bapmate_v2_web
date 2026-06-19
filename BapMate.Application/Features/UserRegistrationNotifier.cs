using System.Threading.Tasks;
using BapMate.Infrastructure.Services;

namespace BapMate.Application.Features
{
    public class UserRegistrationNotifier
    {
        private readonly SmsService _smsService;

        public UserRegistrationNotifier(SmsService smsService)
        {
            _smsService = smsService;
        }

        public async Task NotifyRegistrationAsync(string phoneNumber, string userName)
        {
            var message = $"{userName}님, 밥메이트 회원가입을 환영합니다!";
            await _smsService.SendSmsAsync(phoneNumber, message, "밥메이트 회원가입");
        }

        // 예시: 회원가입 완료 시 호출하는 메서드
        public static async Task SendWelcomeSmsOnRegister(string phoneNumber, string userName)
        {
            // 실제 환경에서는 DI로 SmsService를 주입받아야 함
            var smsService = new SmsService("[user_id]", "[api_key]", "[sender_number]", "[api_url]");
            var notifier = new UserRegistrationNotifier(smsService);
            await notifier.NotifyRegistrationAsync(phoneNumber, userName);
        }
    }
}
