using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Common;

//[Authorize(Roles = "Admin")]
[ApiController]
public class AdminController : ControllerBase
{
}
