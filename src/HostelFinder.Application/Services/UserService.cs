﻿using AutoMapper;
using HostelFinder.Application.DTOs.Users;
using HostelFinder.Application.DTOs.Users.Requests;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HostelFinder.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService
        (
            IMapper mapper,
            IUserRepository userRepository
        )
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<Response<UserDto>> RegisterUserAsync(CreateUserRequestDto request)
        {
            try
            {
                if(await _userRepository.CheckUserNameExistAsync(request.Username))
                {
                    return new Response<UserDto> { Succeeded = false, Message = "User name already exists. Please enter a different user name." };
                }
                if (await _userRepository.CheckEmailExistAsync(request.Email))
                {
                    return new Response<UserDto> { Succeeded = false, Message = "Email already exists. Please enter a different email." };
                }
                if(await _userRepository.CheckPhoneNumberAsync(request.Phone))
                {
                    return new Response<UserDto> { Succeeded = false, Message = "Phone already exists. Please enter a different phone." };
                }

                var userDomain = _mapper.Map<User>(request);

                userDomain.IsDeleted = false;
                userDomain.CreatedOn = DateTime.Now;

                var user = await _userRepository.AddAsync(userDomain);

                var userDto = _mapper.Map<UserDto>(user);

                return new Response<UserDto> { Succeeded = true, Data = userDto, Message = "Add user successfully" }; 
            }
            catch (Exception ex)
            {
                return new Response<UserDto> { Succeeded = false, Errors = {ex.Message}};
            }
        }
    }
}
