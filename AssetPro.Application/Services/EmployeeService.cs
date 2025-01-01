using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace BloodDonation.Application.Services
{
    public class EmployeeService: IEmployeeService
    {
        private readonly IRepository<Employee> _repository;
        private readonly IMapper _mapper;
        public EmployeeService(IRepository<Employee> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public PayloadResponse Create(EmployeeVm model)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<EmployeeVm> GetAll()
        {
            var list = new List<EmployeeVm>();
            var data = _repository.GetAll();
            list = _mapper.Map(data, list);

            return list.AsEnumerable();
        }

        public EmployeeVm GetById(string id)
        {
            throw new System.NotImplementedException();
        }

        public PayloadResponse Update(string id, EmployeeVm model)
        {
            throw new System.NotImplementedException();
        }
    }
}