using System;
using System.Collections.Generic;
using System.Linq;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;

namespace BloodDonation.Application.Services
{
    public class ContactService : IContactService
    {
        public readonly IRepository<Contact> _contactRepository;
        public readonly IRepository<User> _userRepository;

        public ContactService(IRepository<Contact> contactRepository,
            IRepository<User> userRepository)
        {
            _contactRepository = contactRepository;
            _userRepository = userRepository;
        }

        public PayloadResponse Create(Contact contactData)
        {
            try
            {
                _contactRepository.Insert(contactData);
                _contactRepository.SaveChanges();

                return new PayloadResponse()
                {
                    IsSuccess = true,
                    PayloadType = "Contact",
                    Content = null,
                    Message = "Contact placed successfully"
                };
            }
            catch (Exception ex)
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    PayloadType = "Contact",
                    Content = null,
                    Message = $"Contact can't be placed because {ex.Message}"
                };
            }
        }

        public List<ContactVm> GetAll(string contactType, int pageNo, int pageSize)
        {
            var contactList = _contactRepository
                .GetAll()
                .Where(c => c.ContactType == contactType)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var contactWithUserData = (from contact in contactList
                join user in _userRepository.GetAll()
                    on contact.CreatedBy equals user.Id into contactUserData
                from cu in contactUserData
                select new ContactVm()
                {
                    Id = contact.Id,
                    ContactType = contact.ContactType,
                    CreateTime = contact.CreateTime,
                    Subject = contact.Subject,
                    Message = contact.Message,
                    IsRead = contact.IsRead,
                    UserData = new UserCreationVm()
                    {
                        Id = cu.Id,
                        FullName = cu.FullName,
                        UserType = cu.UserType,
                        MobileNumber = cu.MobileNumber,
                        BloodGroup = cu.BloodGroup,
                        Address = cu.Address,
                        Gender = cu.Gender,
                        DateOfBirth = cu.DateOfBirth,
                        ImageUrl = cu.ImageUrl
                    }
                }).ToList();

            return contactWithUserData;
        }

        public PayloadResponse ReadContact(string id)
        {
            try
            {
                var contact = _contactRepository.GetConditional(c => c.Id == id);
                contact.IsRead = true;
                _contactRepository.Update(contact);
                _contactRepository.SaveChanges();

                return new PayloadResponse()
                {
                    IsSuccess = true,
                    PayloadType = "Contact",
                    Content = null,
                    Message = "Contact updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    PayloadType = "Contact",
                    Content = null,
                    Message = $"Contact can't be updated because {ex.Message}"
                };
            }
        }

        public ContactVm Get(string id)
        {
            var contact = _contactRepository
                .GetAll()
                .Where(c => c.Id == id)
                .Select(contact => new ContactVm()
                {
                    Id = contact.Id,
                    ContactType = contact.ContactType,
                    CreateTime = contact.CreateTime,
                    Subject = contact.Subject,
                    Message = contact.Message,
                    IsRead = contact.IsRead,
                    CreatedBy = contact.CreatedBy,
                })
                .FirstOrDefault();

            if (contact is null)
            {
                return new ContactVm();
            }

            var user = _userRepository.GetConditional(u => u.Id == contact.CreatedBy);

            if (user is null) return contact;

            contact.UserData.Id = user.Id;
            contact.UserData.FullName = user.FullName;
            contact.UserData.UserType = user.UserType;
            contact.UserData.MobileNumber = user.MobileNumber;
            contact.UserData.BloodGroup = user.BloodGroup;
            contact.UserData.Address = user.Address;
            contact.UserData.Gender = user.Gender;
            contact.UserData.DateOfBirth = user.DateOfBirth;
            contact.UserData.ImageUrl = user.ImageUrl;

            return contact;
        }
    }
}
