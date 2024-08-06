using ProgramGuard.Enums;

namespace ProgramGuard.Middlewares
{
    public class PrivilegeMiddleware
    {
        private readonly RequestDelegate _next;

        public PrivilegeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 設置所需的權限（根據實際需求調整）
            // 可以根據請求路徑、方法等來動態設置
            var requiredPrivileges = GetRequiredPrivilegesForRequest(context);

            var claim = context.User.Claims.FirstOrDefault(c => c.Type == "operatePrivilege");
            if (claim != null && uint.TryParse(claim.Value, out uint privilege))
            {
                var userPrivileges = (OPERATE_PRIVILEGE)privilege;
                context.Items["OperatePrivilege"] = userPrivileges;

                // 檢查用戶是否擁有所需的權限
                if ((userPrivileges & requiredPrivileges) != requiredPrivileges)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("沒有權限");
                    return;
                }
            }
            else
            {
                context.Items["OperatePrivilege"] = OPERATE_PRIVILEGE.UNKNOWN;
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("沒有權限");
                return;
            }

            await _next(context);
        }

        private OPERATE_PRIVILEGE GetRequiredPrivilegesForRequest(HttpContext context)
        {
            // 根據請求的路徑、方法或其他特徵來確定所需的權限
            // 以下是示例，實際中可以根據需求進行調整
            var path = context.Request.Path.Value.ToLower();

            return path switch
            {
                "/api/add-file" => OPERATE_PRIVILEGE.ADD_FILELIST,
                "/api/modify-file" => OPERATE_PRIVILEGE.MODIFY_FILELIST,
                // 根據請求路徑返回對應的權限
                _ => OPERATE_PRIVILEGE.UNKNOWN
            };
        }
    }
}
