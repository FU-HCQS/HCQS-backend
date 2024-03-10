using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("configuration")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {

        public ConfigurationController()
        {
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ADMIN)]
        [HttpGet("read-app-settings")]
        public IActionResult Get() {

            return Ok(Utility.ReadAppSettingsJson());
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ADMIN)]
        [HttpPost("update-app-settings-value")]
        public IActionResult UpdateAppSettingValue(string section,string key,string value)
        {
            Utility.UpdateAppSettingValue(section, key, value);
            return Ok();
        }
    }
}
