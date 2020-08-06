using AutoMapper;
using MovieLand.Data.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.BLL.Services
{
    public class BaseService
    {
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _unitOfWork;

        public BaseService(IMapper mapper, IUnitOfWork unitOfWork) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
    }
}
