using System.Security.Claims;
using AutoMapper;
using Backend.Common;
using Backend.Data.Domain.Users;
using Backend.Data.Domain.Users.Enum;
using Backend.DTO.Users;
using Backend.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;


public class UserController : PublicController
{

    private readonly IUserService _userService;

    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }


    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

   

    [HttpGet("users/enum/genders")]
    public IActionResult GetGenders()
    {
        var result = EnumHelper.ToList<Gender>();
        return Ok(result);
    }

    [HttpGet("users/enum/address-types")]
    public IActionResult GetAddressTypes()
    {
        var result = EnumHelper.ToList<AddressType>();
        return Ok(result);
    }

    [HttpGet("users/me/addresses")]
    public async Task<IActionResult> GetAllAddress()
    {
        var addresses = await _userService.GetAddressesByUserIdAsync(GetUserId());
        return Ok(_mapper.Map<List<Address>>(addresses));
    }

    [HttpPost("users/me/addresses")]
    public async Task<IActionResult> CreateAddress(CreateAddressRequest request)
    {
        var address = _mapper.Map<Address>(request);
        await _userService.InsertAddressAsync(address);
        return CreatedAtAction(nameof(GetAllAddress), new { id = address.Id }, address);
    }
   
    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetUserAsync(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId); 
        if (user == null)
        {
            return NotFound(new { message = "Người dùng không được tìm thấy " + userId }); 
        }
        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    
}

