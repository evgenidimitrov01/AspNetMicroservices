using System;
using System.Threading.Tasks;
using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories.Abstractions;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DiscountService(IDiscountRepository discountRepository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _discountRepository.GetDiscount(request.ProductName);

            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Discount with product name={request.ProductName} is not found"));
            }

            _logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}",
                coupon.ProductName, coupon.Amount);

            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        #region Overrides of DiscountProtoServiceBase

        /// <inheritdoc />
        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);

            await _discountRepository.CreateDiscount(coupon);

            _logger.LogInformation("Discount is successfully created. ProductName : {productName}", coupon.ProductName);

            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        /// <inheritdoc />
        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);

            await _discountRepository.UpdateDiscount(coupon);

            _logger.LogInformation("Discount is successfully updated. ProductName : {productName}", coupon.ProductName);

            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        /// <inheritdoc />
        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await _discountRepository.DeleteDiscount(request.ProductName);

            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };

            return response;
        }

        #endregion
    }
}
