using Moq;
using Qless.Repositories.Data.DBModels;
using Qless.Repositories.Interfaces;
using Qless.Services.Services;

namespace Qless.UnitTests
{
    [TestClass]
    public class CardTests
    {
        private readonly ICardRepository _cardRepositoryMock = Mock.Of<ICardRepository>();
        private readonly IDiscountRepository _discountRepositoryMock = Mock.Of<IDiscountRepository>();

        private readonly DiscountService _discountService;
        private readonly CardService _cardService;

        private readonly Card _validRegularCard;
        private readonly CardType _cardTypeRegular;
        private readonly CardType _cardTypeDiscounted;
        private readonly CardType _cardTypeDiscountedSpecial;

        private readonly Discount _discountBasic;
        private readonly Discount _discountSucceeding;

        public CardTests()
        {
            _discountService = new DiscountService(_discountRepositoryMock);
            _cardService = new CardService(_cardRepositoryMock, _discountService);

            _cardTypeRegular = new CardType
            {
                Id = 1,
                Name = "regular",
                Description = "",
                BaseRate = 15,
                StartingBalance = 100,
                MaximumIdleDurationYears = 5,
                MaximumBalance = 10000
            };

            _cardTypeDiscounted = new CardType
            {
                Id = 2,
                Name = "discounted",
                Description = "",
                BaseRate = 10,
                StartingBalance = 500,
                MaximumIdleDurationYears = 3,
                MaximumBalance = 10000
            };

            _cardTypeDiscountedSpecial = new CardType
            {
                Id = 3,
                Name = "specialDiscounted",
                Description = "",
                BaseRate = 10,
                StartingBalance = 500,
                MaximumIdleDurationYears = 3,
                MaximumBalance = 10000
            };

            _validRegularCard = new Card
            {
                Id = 1,
                CardTypeId = 1,
                CreatedDate = DateTime.UtcNow,
                LastUsed = null,
                CurrentBalance = 100,
                InTransit = false,
                CardType = _cardTypeRegular
            };

            _discountBasic = new Discount
            {
                Id = 1,
                Name = "base",
                Description = "",
                StartDate = null,
                EndDate = null,
                DiscountValue = 20,
                IsFlatDiscount = false,
                AvailmentLimit = -1,
                AvailmentLimitPerDay = -1,
                Type = "basic"
            };

            _discountSucceeding = new Discount
            {
                Id = 2,
                Name = "succeeding",
                Description = "",
                StartDate = null,
                EndDate = null,
                DiscountValue = 3,
                IsFlatDiscount = false,
                AvailmentLimit = -1,
                AvailmentLimitPerDay = 4,
                Type = "special"
            };
        }

        [TestMethod]
        public async Task EnterStation_ReturnsSuccess_WhenValid()
        {
            // Arrange 
            Mock.Get(_cardRepositoryMock).Setup(x => x.GetCardType(1)).ReturnsAsync(_cardTypeRegular);
            Mock.Get(_cardRepositoryMock).Setup(x => x.UpdateAsync(_validRegularCard)).ReturnsAsync(new Card { InTransit = true });

            // Act
            var result = await _cardService.EnterStation(_validRegularCard);

            // Assert
            Assert.AreEqual(true, result.InTransit);
        }

        [TestMethod]
        public async Task EnterStation_ThrowsError_WhenInTransit()
        {
            try
            {
                // Arrange
                _validRegularCard.InTransit = true;

                Mock.Get(_cardRepositoryMock).Setup(x => x.GetCardType(1)).ReturnsAsync(_cardTypeRegular);
                Mock.Get(_cardRepositoryMock).Setup(x => x.UpdateAsync(_validRegularCard)).ReturnsAsync(new Card { InTransit = true });

                // Act
                var result = await _cardService.EnterStation(_validRegularCard);

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Unable to enter: Card is in transit.", e.Message);
            }
        }

        [TestMethod]
        public async Task EnterStation_ThrowsError_WhenExpired()
        {
            try
            {
                // Arrange
                _validRegularCard.LastUsed = DateTime.Now.AddYears(-5);

                Mock.Get(_cardRepositoryMock).Setup(x => x.GetCardType(1)).ReturnsAsync(_cardTypeRegular);
                Mock.Get(_cardRepositoryMock).Setup(x => x.UpdateAsync(_validRegularCard)).ReturnsAsync(new Card { InTransit = true });

                // Act
                var result = await _cardService.EnterStation(_validRegularCard);

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Unable to enter: Card is expired.", e.Message);
            }
        }

        [TestMethod]
        public async Task EnterStation_ThrowsError_InsufficientBalance()
        {
            try
            {
                // Arrange
                _validRegularCard.CurrentBalance = 10;

                Mock.Get(_cardRepositoryMock).Setup(x => x.GetCardType(1)).ReturnsAsync(_cardTypeRegular);
                Mock.Get(_cardRepositoryMock).Setup(x => x.UpdateAsync(_validRegularCard)).ReturnsAsync(new Card { InTransit = true });

                // Act
                var result = await _cardService.EnterStation(_validRegularCard);

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Unable to enter: Insufficient Balance.", e.Message);
            }
        }

        [TestMethod]
        public async Task ExitStation_ThrowsError_IfNotInTransit()
        {
            try
            {
                // Arrange
                _validRegularCard.InTransit = false;

                Mock.Get(_cardRepositoryMock).Setup(x => x.GetCardType(1)).ReturnsAsync(_cardTypeRegular);
                Mock.Get(_cardRepositoryMock).Setup(x => x.UpdateAsync(_validRegularCard)).ReturnsAsync(new Card { InTransit = true });

                // Act
                var result = await _cardService.ExitStation(_validRegularCard);

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Card not in transit.", e.Message);
            }
        }

        [TestMethod]
        public async Task ExitStation_ThrowsError_IfExpired()
        {
            try
            {
                // Arrange
                _validRegularCard.LastUsed = DateTime.Now.AddYears(-5);

                Mock.Get(_cardRepositoryMock).Setup(x => x.GetCardType(1)).ReturnsAsync(_cardTypeRegular);
                Mock.Get(_cardRepositoryMock).Setup(x => x.UpdateAsync(_validRegularCard)).ReturnsAsync(new Card { InTransit = true });

                // Act
                var result = await _cardService.ExitStation(_validRegularCard);

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Card not in transit.", e.Message);
            }
        }

        [TestMethod]
        public async Task ExitStation_ReturnsSuccess_WhenValid()
        {
            // Arrange
            _validRegularCard.InTransit = true;

            var list = new List<Discount>();
            list.Add(_discountBasic);
            list.Add(_discountSucceeding);

            Mock.Get(_discountRepositoryMock).Setup(x => x.GetAllAsync(_validRegularCard.CardTypeId)).ReturnsAsync(list);
            Mock.Get(_cardRepositoryMock).Setup(x => x.GetCardType(1)).ReturnsAsync(_cardTypeRegular);
            Mock.Get(_cardRepositoryMock).Setup(x => x.UpdateAsync(_validRegularCard)).ReturnsAsync(new Card());

            // Act
            var result = await _cardService.ExitStation(_validRegularCard);

            // Assert
            Assert.AreEqual(85, _validRegularCard.CurrentBalance);
        }

        [TestMethod]
        public async Task LoadCard_ReturnsSuccess_WhenValid()
        {
            // Arrange
            var id = 1;
            Mock.Get(_cardRepositoryMock).Setup(x => x.GetAsync(id)).ReturnsAsync(_validRegularCard);
            Mock.Get(_cardRepositoryMock).Setup(x => x.UpdateAsync(_validRegularCard)).ReturnsAsync(new Card());

            // Act
            var result = await _cardService.LoadCard(id, 1100);

            // Assert
            Assert.AreEqual(1100, _validRegularCard.CurrentBalance);
        }

        [TestMethod]
        public async Task LoadCard_ThrowsError_WhenLimitExceeded()
        {
            try
            {
                // Arrange
                var id = 1;
                Mock.Get(_cardRepositoryMock).Setup(x => x.GetAsync(id)).ReturnsAsync(_validRegularCard);
                Mock.Get(_cardRepositoryMock).Setup(x => x.UpdateAsync(_validRegularCard)).ReturnsAsync(new Card());

                // Act
                var result = await _cardService.LoadCard(id, 11100);

                // Assert
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual($"Card balance limit must not exceed {_validRegularCard.CardType.MaximumBalance}", e.Message);
            }
        }

        [TestMethod]
        public async Task LoadCard_ThrowsError_WhenNewBalanceIsLowerThanCurrentBalance()
        {
            try
            {
                // Arrange
                var id = 1;
                Mock.Get(_cardRepositoryMock).Setup(x => x.GetAsync(id)).ReturnsAsync(_validRegularCard);
                Mock.Get(_cardRepositoryMock).Setup(x => x.UpdateAsync(_validRegularCard)).ReturnsAsync(new Card());

                // Act
                var result = await _cardService.LoadCard(id, 90);

                // Assert
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("New balance cant be less than current balance.", e.Message);
            }
        }

        [TestMethod]
        public async Task ComputeCharges_IsCorrect_WhenValidSpecialCard_FirstUse()
        {
            // Arrange
            _validRegularCard.InTransit = true;
            _validRegularCard.CurrentBalance = 100;
            _validRegularCard.CardTypeId = 3;
            _validRegularCard.CardType = _cardTypeDiscountedSpecial;
            _validRegularCard.TripCountToday = 0;
            _validRegularCard.LastUsed = null;

            var list = new List<Discount>();
            list.Add(_discountBasic);
            list.Add(_discountSucceeding);

            Mock.Get(_discountRepositoryMock).Setup(x => x.GetAllAsync(_validRegularCard.CardTypeId)).ReturnsAsync(list);

            // Act
            await _discountService.ComputeCharges(_validRegularCard);

            // Assert
            Assert.AreEqual(92, _validRegularCard.CurrentBalance);
        }

        [TestMethod]
        public async Task ComputeCharges_IsCorrect_WhenValidSpecialCard_SecondUse()
        {
            // Arrange
            _validRegularCard.InTransit = true;
            _validRegularCard.CurrentBalance = 100;
            _validRegularCard.CardTypeId = 3;
            _validRegularCard.CardType = _cardTypeDiscountedSpecial;
            _validRegularCard.TripCountToday = 1;
            _validRegularCard.LastUsed = DateTime.Now;

            var list = new List<Discount>();
            list.Add(_discountBasic);
            list.Add(_discountSucceeding);

            Mock.Get(_discountRepositoryMock).Setup(x => x.GetAllAsync(_validRegularCard.CardTypeId)).ReturnsAsync(list);

            // Act
            await _discountService.ComputeCharges(_validRegularCard);

            // Assert
            Assert.AreEqual((decimal)92.30, _validRegularCard.CurrentBalance);
        }

        [TestMethod]
        public async Task ComputeCharges_IsCorrect_WhenValidSpecialCard_FifthUse()
        {
            // Arrange
            _validRegularCard.InTransit = true;
            _validRegularCard.CurrentBalance = 100;
            _validRegularCard.CardTypeId = 3;
            _validRegularCard.CardType = _cardTypeDiscountedSpecial;
            _validRegularCard.TripCountToday = 5;
            _validRegularCard.LastUsed = DateTime.Now;

            var list = new List<Discount>();
            list.Add(_discountBasic);
            list.Add(_discountSucceeding);

            Mock.Get(_discountRepositoryMock).Setup(x => x.GetAllAsync(_validRegularCard.CardTypeId)).ReturnsAsync(list);

            // Act
            await _discountService.ComputeCharges(_validRegularCard);

            // Assert
            Assert.AreEqual((decimal)92.0, _validRegularCard.CurrentBalance);
        }

        [TestMethod]
        public async Task ComputeCharges_IsCorrect_WhenValidSpecialCard_SixthUse()
        {
            // Arrange
            _validRegularCard.InTransit = true;
            _validRegularCard.CurrentBalance = 100;
            _validRegularCard.CardTypeId = 3;
            _validRegularCard.CardType = _cardTypeDiscountedSpecial;
            _validRegularCard.TripCountToday = 6;
            _validRegularCard.LastUsed = DateTime.Now;

            var list = new List<Discount>();
            list.Add(_discountBasic);
            list.Add(_discountSucceeding);

            Mock.Get(_discountRepositoryMock).Setup(x => x.GetAllAsync(_validRegularCard.CardTypeId)).ReturnsAsync(list);

            // Act
            await _discountService.ComputeCharges(_validRegularCard);

            // Assert
            Assert.AreEqual((decimal)92.0, _validRegularCard.CurrentBalance);
        }

        [TestMethod]
        public async Task ComputeCharges_IsCorrect_WhenValidSpecialCard_SixthUse_NextDay()
        {
            // Arrange
            _validRegularCard.InTransit = true;
            _validRegularCard.CurrentBalance = 100;
            _validRegularCard.CardTypeId = 3;
            _validRegularCard.CardType = _cardTypeDiscountedSpecial;
            _validRegularCard.TripCountToday = 6;
            _validRegularCard.LastUsed = DateTime.Now.AddDays(-1);

            var list = new List<Discount>();
            list.Add(_discountBasic);
            list.Add(_discountSucceeding);

            Mock.Get(_discountRepositoryMock).Setup(x => x.GetAllAsync(_validRegularCard.CardTypeId)).ReturnsAsync(list);

            // Act
            await _discountService.ComputeCharges(_validRegularCard);

            // Assert
            Assert.AreEqual((decimal)92.0, _validRegularCard.CurrentBalance);
        }
    }
}
