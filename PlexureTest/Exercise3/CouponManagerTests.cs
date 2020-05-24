using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PlexureTest.Exercise3
{
    [TestClass]
    public class CouponManagerTests
    {
        [TestMethod]
        public async Task CanRedeemCoupon_RetrievesCoupon()
        {
            //Arrange
            var loggerMock = new Mock<ILogger>();
            var couponProviderMock = new Mock<ICouponProvider>();
            couponProviderMock.Setup(x => x.Retrieve(It.IsAny<Guid>()))
                .ReturnsAsync(new Coupon());
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var controller = new CouponManager(loggerMock.Object, couponProviderMock.Object);

            //Act
            await controller.CanRedeemCoupon(couponId, userId, evaluators: Enumerable.Empty<Func<Coupon, Guid, bool>>());

            //Assert
            couponProviderMock.Verify(x => x.Retrieve(couponId), Times.Once);
        }

        [TestMethod]
        public async Task CanRedeemCoupon_ReturnsTrue_IfNoEvaluators()
        {
            //Arrange
            var loggerMock = new Mock<ILogger>();
            var couponProviderMock = new Mock<ICouponProvider>();
            couponProviderMock.Setup(x => x.Retrieve(It.IsAny<Guid>()))
                .ReturnsAsync(new Coupon());
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var controller = new CouponManager(loggerMock.Object, couponProviderMock.Object);

            //Act
            var result = await controller.CanRedeemCoupon(couponId, userId, evaluators: Enumerable.Empty<Func<Coupon, Guid, bool>>());

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task CanRedeemCoupon_ThrowsException_IfCouponNotFound()
        {
            //Arrange
            var loggerMock = new Mock<ILogger>();
            var couponProviderMock = new Mock<ICouponProvider>();
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var controller = new CouponManager(loggerMock.Object, couponProviderMock.Object);
            //Act and Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                async () =>
                {
                    await controller.CanRedeemCoupon(couponId, userId,
                        evaluators: Enumerable.Empty<Func<Coupon, Guid, bool>>());
                }
            );
        }

        [TestMethod]
        public async Task CanRedeemCoupon_ThrowsException_IfEvaluatorsIsNull()
        {
            //Arrange
            var loggerMock = new Mock<ILogger>();
            var couponProviderMock = new Mock<ICouponProvider>();
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var controller = new CouponManager(loggerMock.Object, couponProviderMock.Object);
            //Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                async () =>
                {
                    await controller.CanRedeemCoupon(couponId, userId,null);
                }
            );
        }

        [TestMethod]
        public async Task CanRedeemCoupon_ReturnsTrue_IfOneEvaluatorReturnsTrue()
        {
            //Arrange
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var loggerMock = new Mock<ILogger>();
            var couponProviderMock = new Mock<ICouponProvider>();
            couponProviderMock.Setup(x => x.Retrieve(couponId))
                .ReturnsAsync(new Coupon());

            var evaluator1Mock = new Mock<Func<Coupon, Guid, bool>>();
            evaluator1Mock.Setup(x => x.Invoke(It.IsAny<Coupon>(), userId)).Returns(true);

            var evaluator2Mock = new Mock<Func<Coupon, Guid, bool>>();
            evaluator2Mock.Setup(x => x.Invoke(It.IsAny<Coupon>(), userId)).Returns(false);

            var controller = new CouponManager(loggerMock.Object, couponProviderMock.Object);

            //Act
            var result = await controller.CanRedeemCoupon(couponId, userId,
                evaluators: new[] {evaluator1Mock.Object, evaluator2Mock.Object});

            //Assert
            Assert.AreEqual(true, result);
            evaluator1Mock.Verify(x => x.Invoke(It.IsAny<Coupon>(), userId), Times.Once);
            evaluator2Mock.Verify(x => x.Invoke(It.IsAny<Coupon>(), userId), Times.Once);
        }
        [TestMethod]
        public async Task CanRedeemCoupon_ReturnsFalse_IfAllEvaluatorsReturnFalse()
        {
            //Arrange
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var loggerMock = new Mock<ILogger>();
            var couponProviderMock = new Mock<ICouponProvider>();
            couponProviderMock.Setup(x => x.Retrieve(couponId))
                .ReturnsAsync(new Coupon());

            var evaluator1Mock = new Mock<Func<Coupon, Guid, bool>>();
            evaluator1Mock.Setup(x => x.Invoke(It.IsAny<Coupon>(), userId)).Returns(false);

            var evaluator2Mock = new Mock<Func<Coupon, Guid, bool>>();
            evaluator2Mock.Setup(x => x.Invoke(It.IsAny<Coupon>(), userId)).Returns(false);

            var controller = new CouponManager(loggerMock.Object, couponProviderMock.Object);

            //Act
            var result = await controller.CanRedeemCoupon(couponId, userId,
                evaluators: new[] { evaluator1Mock.Object, evaluator2Mock.Object });

            //Assert
            Assert.AreEqual(false, result);
            evaluator1Mock.Verify(x => x.Invoke(It.IsAny<Coupon>(), userId), Times.Once);
            evaluator2Mock.Verify(x => x.Invoke(It.IsAny<Coupon>(), userId), Times.Once);
        }
    }
}
