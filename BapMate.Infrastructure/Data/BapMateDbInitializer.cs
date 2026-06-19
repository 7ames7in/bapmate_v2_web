using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using BapMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BapMate.Infrastructure.Data;

public static class BapMateDbInitializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private static readonly DateTime BaseDate = new(2025, 1, 1, 9, 0, 0, DateTimeKind.Unspecified);

    private static readonly string[] TimeSlots =
    {
        "breakfast",
        "brunch",
        "lunch",
        "tea-time",
        "dinner",
        "late-night",
    };

    private static readonly string[] PaymentTypes =
    {
        "split",
        "host-pay",
        "guest-pay",
        "roulette",
    };

    private static readonly string[] Interests =
    {
        "한식",
        "중식",
        "일식",
        "양식",
        "비건",
        "디저트",
        "맛집 탐방",
        "네트워킹",
        "야식",
        "브런치",
    };

    private static readonly string[] MissionPool =
    {
        "결제 담당",
        "자리 예약",
        "후기 작성",
        "사진 담당",
        "쿠폰 찾기",
        "주차 확인",
        "디저트 담당",
    };

    private static readonly string[] CostOptions =
    {
        "0",
        "5,000",
        "10,000",
        "15,000",
        "20,000",
    };

    private static readonly string[] SampleCities =
    {
        "서울",
        "성남",
        "부산",
        "대구",
        "광주",
        "대전",
        "인천",
        "수원",
        "제주",
        "춘천",
    };

    private static readonly string[] SampleDistricts =
    {
        "강남구",
        "마포구",
        "성동구",
        "분당구",
        "해운대구",
        "수성구",
        "동구",
        "중구",
        "서구",
        "남구",
    };

    private static readonly string[] CuisineCategories =
    {
        "한식",
        "일식",
        "중식",
        "양식",
        "분식",
        "카페",
        "퓨전",
    };

    private static readonly string[] SupportThemes =
    {
        "지역 맛집 콘텐츠 제작",
        "청년 셰프 팝업 스토어",
        "농산물 직거래 장터",
        "푸드트럭 페스티벌",
        "공유 주방 창업 지원",
        "지역 특산물 홍보",
    };

    private static readonly string[] GroupCostRules =
    {
        "roulette",
        "ladder",
        "weighted",
        "equal",
    };

    private static readonly string[] GroupVisibilityOptions =
    {
        "public",
        "private",
    };

    private static readonly string[] NotificationTypes =
    {
        "group",
        "support",
        "game",
        "app",
    };

    private static readonly string[] FestivalCategories =
    {
        "food",
        "culture",
        "music",
        "market",
    };

    private static readonly string[] FestivalOrganizers =
    {
        "밥메이트 스튜디오",
        "지역 상생 위원회",
        "푸드앤컬처 협동조합",
        "로컬페어",
    };

    public static async Task InitializeAsync(BapMateDbContext context, CancellationToken cancellationToken = default)
    {
        await context.Database.EnsureCreatedAsync(cancellationToken);

        if (await context.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var users = CreateUsers();
        var friends = CreateFriends();
        var supportRequests = CreateSupportRequests();
        var groups = CreateGroups();
        var restaurants = CreateRestaurants();
        var paymentTransactions = CreatePaymentTransactions(users);
        var notifications = CreateNotifications();
        var chatThreads = CreateChatThreads();
        var gameHistories = CreateGameHistories();
        var festivals = CreateFestivals();
        var matchRequests = CreateMatchRequests();
        var directoryEntries = CreateUserDirectoryEntries();
        var restaurantDirectory = CreateRestaurantDirectory();

        await context.Users.AddRangeAsync(users, cancellationToken);
        await context.Friends.AddRangeAsync(friends, cancellationToken);
        await context.SupportRequests.AddRangeAsync(supportRequests, cancellationToken);
        await context.Groups.AddRangeAsync(groups, cancellationToken);
        await context.Restaurants.AddRangeAsync(restaurants, cancellationToken);
        await context.PaymentTransactions.AddRangeAsync(paymentTransactions, cancellationToken);
        await context.Notifications.AddRangeAsync(notifications, cancellationToken);
        await context.ChatThreads.AddRangeAsync(chatThreads, cancellationToken);
        await context.GameHistories.AddRangeAsync(gameHistories, cancellationToken);
        await context.Festivals.AddRangeAsync(festivals, cancellationToken);
        await context.MatchRequests.AddRangeAsync(matchRequests, cancellationToken);
        await context.UserDirectory.AddRangeAsync(directoryEntries, cancellationToken);
        await context.RestaurantDirectory.AddRangeAsync(restaurantDirectory, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }



    private static IReadOnlyCollection<User> CreateUsers() =>
        Enumerable.Range(1, 30)
            .Select(index =>
            {
                var name = UserName(index);
                var badges = new[]
                {
                    index % 3 == 0 ? "시간 약속 100%" : "밥친구 메이트",
                    index % 4 == 0 ? "후기 마스터" : "맛집 헌터",
                    index % 5 == 0 ? "밥 사준 경험" : "정산 도우미",
                }.Distinct().ToArray();

                var preferredSlots = new[]
                    {
                        TimeSlots[(index - 1) % TimeSlots.Length],
                        TimeSlots[index % TimeSlots.Length],
                        TimeSlots[(index + 2) % TimeSlots.Length],
                    }
                    .Distinct()
                    .Take(3)
                    .ToArray();

                var preferredPayments = new[]
                    {
                        PaymentTypes[(index - 1) % PaymentTypes.Length],
                        PaymentTypes[(index + 1) % PaymentTypes.Length],
                    }
                    .Distinct()
                    .ToArray();

                var preferredInterests = Interests
                    .Skip(index % Interests.Length)
                    .Concat(Interests.Take(3))
                    .Take(3)
                    .ToArray();

                var missions = MissionPool
                    .Skip(index % MissionPool.Length)
                    .Concat(MissionPool)
                    .Take(3)
                    .ToArray();

                var costs = CostOptions
                    .Skip(index % CostOptions.Length)
                    .Concat(CostOptions)
                    .Take(3)
                    .ToArray();

                return new User
                {
                    Id = UserId(index),
                    Name = name,
                    Email = $"sample{index:D2}@bapmate.app",
                    Avatar = AvatarFor(index),
                    Bio = $"{SampleCities[(index - 1) % SampleCities.Length]}에서 활동하는 맛집 탐험가예요.",
                    ReliabilityScore = 60 + (index % 40),
                    WalletBalance = 20000 + (index * 1500),
                    EscrowBalance = 5000 + ((index % 5) * 2500),
                    BadgesJson = Json(badges),
                    MatchPreferencesJson = Json(new
                    {
                        preferredTimeSlots = preferredSlots,
                        preferredPaymentTypes = preferredPayments,
                        preferredInterests,
                    }),
                    DefaultGameSettingsJson = Json(new
                    {
                        defaultMissions = missions,
                        defaultCosts = costs,
                    }),
                };
            })
            .ToArray();

    private static IReadOnlyCollection<PaymentTransaction> CreatePaymentTransactions(IReadOnlyCollection<User> users)
    {
        var transactions = new List<PaymentTransaction>();

        foreach (var user in users)
        {
            if (!int.TryParse(user.Id.Split('-').Last(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var userIndex))
            {
                userIndex = 1;
            }

            var currentWallet = user.WalletBalance;
            var currentEscrow = user.EscrowBalance;
            var sequenceNumber = 0;
            var createdAt = BaseDate.AddDays(userIndex).AddHours(18);

            void AddTransaction(string title, string type, string category, decimal walletDelta, decimal escrowDelta, string? memo, string? counterparty)
            {
                if (walletDelta == 0 && escrowDelta == 0)
                {
                    return;
                }

                var direction = DetermineFlow(walletDelta, escrowDelta);
                var amount = walletDelta != 0 ? Math.Abs(walletDelta) : Math.Abs(escrowDelta);

                transactions.Add(new PaymentTransaction
                {
                    Id = PaymentTransactionId(userIndex, ++sequenceNumber),
                    UserId = user.Id,
                    Title = title,
                    Category = category,
                    Type = type,
                    Direction = direction,
                    Amount = amount,
                    WalletDelta = walletDelta,
                    EscrowDelta = escrowDelta,
                    WalletBalanceAfter = currentWallet,
                    EscrowBalanceAfter = currentEscrow,
                    Currency = "KRW",
                    Counterparty = counterparty,
                    Memo = memo,
                    CreatedAt = createdAt,
                });

                currentWallet -= walletDelta;
                currentEscrow -= escrowDelta;
                createdAt = createdAt.AddHours(-8);
            }

            decimal SafeDebit(decimal suggested)
            {
                var max = Math.Max(currentWallet - 2000m, 1200m);
                return Math.Min(suggested, max);
            }

            decimal SafeEscrowHold(decimal suggested)
            {
                var walletRoom = Math.Max(currentWallet - 3000m, 1500m);
                var escrowRoom = Math.Max(currentEscrow - 1000m, 1000m);
                return Math.Min(suggested, Math.Min(walletRoom, escrowRoom));
            }

            var diningCost = SafeDebit(4500m + (userIndex % 3) * 900m);
            AddTransaction("모임 정산", "group_payment", "group", -diningCost, 0m, "강남 직장인 점심", "분당 미식회");

            var recharge = Math.Min(currentWallet - 2500m, 14000m + (userIndex % 4) * 2000m);
            if (recharge > 2000m)
            {
                AddTransaction("빠른 충전", "topup", "topup", recharge, 0m, "KB국민 1029", "내 통장");
            }

            var escrowRefund = Math.Min(currentEscrow, 1500m + (userIndex % 2) * 1200m);
            if (escrowRefund > 0m)
            {
                AddTransaction("예치금 환급", "escrow_release", "escrow", escrowRefund, -escrowRefund, "참석 확인 완료", "밥메이트");
            }

            var escrowHold = SafeEscrowHold(2200m + (userIndex % 3) * 800m);
            if (escrowHold > 0m)
            {
                AddTransaction("예치금 보류", "escrow_hold", "escrow", -escrowHold, escrowHold, "신촌 번개 모임", "밥메이트");
            }

            var reward = Math.Min(currentWallet - 2000m, 1800m + (userIndex % 3) * 200m);
            if (reward > 0m)
            {
                AddTransaction("후기 리워드", "reward", "reward", reward, 0m, "후기 2건 완료", "밥메이트");
            }

            var donation = Math.Min(currentWallet - 2500m, 2400m + (userIndex % 2) * 600m);
            if (donation > 800m)
            {
                AddTransaction("지역 기부", "donation", "support", -donation, 0m, "푸드트럭 페스티벌", "밥메이트 기금");
            }
        }

        return transactions;
    }



    private static string DetermineFlow(decimal walletDelta, decimal escrowDelta)
    {
        var hasWalletChange = walletDelta != 0;
        var hasEscrowChange = escrowDelta != 0;

        if (hasWalletChange && hasEscrowChange)
        {
            if (walletDelta > 0 && escrowDelta < 0)
            {
                return "transfer-in";
            }

            if (walletDelta < 0 && escrowDelta > 0)
            {
                return "transfer-out";
            }

            return "transfer";
        }

        if (walletDelta > 0 || escrowDelta > 0)
        {
            return "credit";
        }

        return "debit";
    }

    private static IReadOnlyCollection<Friend> CreateFriends() =>
        Enumerable.Range(1, 30)
            .Select(index =>
            {
                var ownerIndex = ((index - 1) % 10) + 1;
                var friendIndex = ((index + 9) % 30) + 1;
                var tagOptions = new[]
                {
                    "점심러",
                    "야식러",
                    "네트워킹",
                    "맛집 공유",
                    "브런치",
                };

                var tags = tagOptions
                    .Skip(index % tagOptions.Length)
                    .Concat(tagOptions)
                    .Take(2)
                    .ToArray();

                return new Friend
                {
                    Id = FriendId(index),
                    OwnerId = UserId(ownerIndex),
                    Name = UserName(friendIndex),
                    Avatar = AvatarFor(friendIndex),
                    TrustLevel = 58 + (index % 40),
                    LastMeal = DateOnlyString(index * 2),
                    TagsJson = Json(tags),
                    Memo = $"최근 {SampleCities[(index - 1) % SampleCities.Length]} {SampleDistricts[(index - 1) % SampleDistricts.Length]} 맛집 투어에 함께했어요.",
                    Phone = PhoneFor(index),
                    Identifier = $"mate{index:D2}",
                };
            })
            .ToArray();

    private static IReadOnlyCollection<SupportRequest> CreateSupportRequests() =>
        Enumerable.Range(1, 30)
            .Select(index =>
            {
                var theme = SupportThemes[(index - 1) % SupportThemes.Length];
                var city = SampleCities[(index - 1) % SampleCities.Length];
                var progress = Math.Min(0.95, 0.2 + ((index % 7) * 0.1));

                return new SupportRequest
                {
                    Id = SupportId(index),
                    Title = $"{city} {theme} #{index:D2}",
                    Story = $"{city} 지역에서 진행하는 '{theme}' 프로젝트에 힘을 보태주세요!",
                    Amount = 150000 + (index * 25000),
                    Verified = index % 3 == 0,
                    Progress = Math.Round(progress, 2, MidpointRounding.AwayFromZero),
                    ExpiresAt = DateTimeIsoString(20 + index, 17),
                };
            })
            .ToArray();

    private static IReadOnlyCollection<Group> CreateGroups() =>
        Enumerable.Range(1, 30)
            .Select(index =>
            {
                var hostIndex = ((index - 1) % 10) + 1;
                var participantIndexes = new[]
                {
                    hostIndex,
                    ((index + 5) % 30) + 1,
                    ((index + 12) % 30) + 1,
                };

                var participants = participantIndexes
                    .Select((participantIndex, order) => new
                    {
                        id = UserId(participantIndex),
                        name = UserName(participantIndex),
                        avatar = AvatarFor(participantIndex),
                        status = order == 0 ? "confirmed" : order == 1 ? "confirmed" : "pending",
                        intention = order == 0 ? "이번 모임 호스트" : order == 1 ? "맛집 탐방" : "시간 조율 중",
                        likabilityScore = Math.Min(98, 60 + (order == 0 ? 25 : order == 1 ? 12 : 6) + (participantIndex % 7)),
                    })
                    .ToArray();

                var visibility = GroupVisibilityOptions[index % GroupVisibilityOptions.Length];
                var costMethod = index % 3 == 0 ? "ratio" : "equal";
                var billRatios = costMethod == "ratio"
                    ? Json(new Dictionary<string, int>
                    {
                        [UserId(hostIndex)] = 2,
                        [UserId(((index + 5) % 30) + 1)] = 1,
                        [UserId(((index + 12) % 30) + 1)] = 1,
                    })
                    : null;

                var reviews = index % 2 == 0
                    ? Json(new[]
                    {
                    new
                    {
                        id = $"review-{index:000}",
                        userId = UserId(((index + 5) % 30) + 1),
                        userName = UserName(((index + 5) % 30) + 1),
                        userAvatar = AvatarFor(((index + 5) % 30) + 1),
                        rating = 4 + (index % 2),
                        comment = "모임 분위기가 좋았고 정산도 깔끔했어요!",
                        date = DateOnlyString(index + 3),
                        helpful = 2 + (index % 5),
                    },
                    })
                    : Json(Array.Empty<object>());

                return new Group
                {
                    Id = GroupId(index),
                    Title = $"{SampleCities[(index - 1) % SampleCities.Length]} {CuisineCategories[(index - 1) % CuisineCategories.Length]} 모임 {index:D2}",
                    Description = $"{SampleCities[(index - 1) % SampleCities.Length]}에서 {CuisineCategories[(index - 1) % CuisineCategories.Length]}을(를) 함께 즐겨요.",
                    Location = $"{SampleCities[(index - 1) % SampleCities.Length]} {SampleDistricts[(index - 1) % SampleDistricts.Length]}",
                    Time = DateTimeLocalString(index * 2, 19, 0),
                    CreatedAt = DateTimeIsoString(index, 9),
                    MinMembers = 2 + (index % 3),
                    MaxMembers = 5 + (index % 4),
                    CostRule = GroupCostRules[(index - 1) % GroupCostRules.Length],
                    Visibility = visibility,
                    HostId = UserId(hostIndex),
                    RestaurantId = RestaurantId(((index - 1) % 30) + 1),
                    BillSplitMethod = costMethod,
                    BillSplitRatiosJson = billRatios,
                    BillSplitGameId = string.Equals(costMethod, "game", StringComparison.OrdinalIgnoreCase) ? GameId(index) : null,
                    ParticipantsJson = Json(participants),
                    ReviewsJson = reviews,
                };
            })
            .ToArray();

    private static IReadOnlyCollection<Restaurant> CreateRestaurants() =>
        Enumerable.Range(1, 30)
            .Select(static index =>
            {
                var city = SampleCities[(index - 1) % SampleCities.Length];
                var district = SampleDistricts[(index - 1) % SampleDistricts.Length];
                var category = CuisineCategories[(index - 1) % CuisineCategories.Length];
                var rating = Math.Min(4.9, 3.7 + ((index % 12) * 0.1));

                var menu = new[]
                {
                    new
                    {
                        id = $"menu-{index:000}-1",
                        name = $"{category} 대표 메뉴",
                        price = 12000 + (index * 400),
                        category = "메인",
                        description = $"{city}에서 인기 있는 {category} 대표 메뉴예요.",
                        popular = index % 2 == 0,
                        rating = rating,
                        totalReviews = 6 + (index % 4),
                        tags = new[] { "대표", "베스트" },
                        reviews = new[]
                        {
                            new
                            {
                                id = $"menu-review-{index:000}-1",
                                userId = UserId(((index + 1) % 30) + 1),
                                userName = UserName(((index + 1) % 30) + 1),
                                userAvatar = AvatarFor(((index + 1) % 30) + 1),
                                rating = Math.Min(5.0, rating + 0.2),
                                comment = $"{category} 고유의 풍미가 살아있어요. 다음에도 꼭 주문할 것 같아요!",
                                date = DateOnlyString(index + 2),
                                helpful = 4 + (index % 3),
                                images = new[] { $"https://picsum.photos/seed/menu{index}a/300" },
                            },
                            new
                            {
                                id = $"menu-review-{index:000}-2",
                                userId = UserId(((index + 5) % 30) + 1),
                                userName = UserName(((index + 5) % 30) + 1),
                                userAvatar = AvatarFor(((index + 5) % 30) + 1),
                                rating = Math.Max(3.5, rating - 0.1),
                                comment = "가성비가 좋아요. 양도 넉넉해서 모임 메뉴로 추천합니다.",
                                date = DateOnlyString(index + 3),
                                helpful = 2 + (index % 4),
                                images = Array.Empty<string>(),
                            },
                            new
                            {
                                id = $"menu-review-{index:000}-3",
                                userId = UserId(((index + 9) % 30) + 1),
                                userName = UserName(((index + 9) % 30) + 1),
                                userAvatar = AvatarFor(((index + 9) % 30) + 1),
                                rating = Math.Min(5.0, rating + 0.1),
                                comment = "사진 그대로의 비주얼이에요. 밥메이트 모임에서도 인기 메뉴였어요.",
                                date = DateOnlyString(index + 4),
                                helpful = 1 + (index % 2),
                                images = Array.Empty<string>(),
                            },
                        },
                    },
                    new
                    {
                        id = $"menu-{index:000}-2",
                        name = $"{category} 시그니처",
                        price = 15000 + (index * 300),
                        category = "스페셜",
                        description = "호스트 추천 메뉴로 인기가 많아요.",
                        popular = index % 3 == 0,
                        rating = rating - 0.1,
                        totalReviews = 3 + (index % 3),
                        tags = new[] { "시그니처", "추천" },
                        reviews = new[]
                        {
                            new
                            {
                                id = $"menu-review-{index:000}-4",
                                userId = UserId(((index + 2) % 30) + 1),
                                userName = UserName(((index + 2) % 30) + 1),
                                userAvatar = AvatarFor(((index + 2) % 30) + 1),
                                rating = Math.Min(5.0, rating + 0.1),
                                comment = "모임 호스트 추천답게 다들 만족했어요!",
                                date = DateOnlyString(index + 1),
                                helpful = 3 + (index % 3),
                                images = Array.Empty<string>(),
                            },
                            new
                            {
                                id = $"menu-review-{index:000}-5",
                                userId = UserId(((index + 7) % 30) + 1),
                                userName = UserName(((index + 7) % 30) + 1),
                                userAvatar = AvatarFor(((index + 7) % 30) + 1),
                                rating = Math.Max(3.5, rating - 0.2),
                                comment = "조금 특별한 메뉴를 찾을 때 좋아요.",
                                date = DateOnlyString(index + 5),
                                helpful = 1 + (index % 3),
                                images = Array.Empty<string>(),
                            },
                        },
                    },
                };

                var reviews = new[]
                {
                    new
                    {
                        id = $"rest-review-{index:000}",
                        userId = UserId(((index + 4) % 30) + 1),
                        userName = UserName(((index + 4) % 30) + 1),
                        userAvatar = AvatarFor(((index + 4) % 30) + 1),
                        rating = rating,
                        comment = $"{city} {district}에서 꼭 가봐야 할 {category} 맛집이에요!",
                        date = DateOnlyString(index + 5),
                        helpful = 3 + (index % 4),
                        images = new[] { $"https://picsum.photos/seed/rest{index}/300" },
                    },
                };

                var coupons = new[]
                {
                    new
                    {
                        id = $"coupon-{index:000}",
                        title = "신규 방문 10% 할인",
                        condition = "주중 17시 이전 방문",
                        expiresAt = DateOnlyString(index + 30),
                    },
                };

                return new Restaurant
                {
                    Id = RestaurantId(index),
                    Name = $"{city} {category} 맛집 {index:D2}",
                    Category = category,
                    Rating = rating,
                    Distance = $"{450 + (index * 12)}m",
                    Address = $"{city} {district} 맛집로 {10 + index}",
                    Latitude = 37.4 + (index * 0.01),
                    Longitude = 126.8 + (index * 0.008),
                    MenuJson = Json(menu),
                    ReviewsJson = Json(reviews),
                    CouponsJson = Json(coupons),
                    EventHistoryJson = Json(new[]
                    {
                        new
                        {
                            id = $"event-{index:000}",
                            title = "주말 한정 스페셜",
                            description = $"{category} 세트 주문 시 음료 서비스",
                            date = DateOnlyString(index + 12),
                        },
                    }),
                    TotalReviews = 120 + (index * 5),
                    PriceRange = index % 4 == 0 ? "₩₩₩" : "₩₩",
                    BusinessHours = "11:00-22:00",
                    Phone = $"02-55{index:000}",
                };
            })
            .ToArray();

    private static IReadOnlyCollection<Notification> CreateNotifications() =>
        Enumerable.Range(1, 30)
            .Select(index =>
            {
                var type = NotificationTypes[(index - 1) % NotificationTypes.Length];
                var title = type switch
                {
                    "group" => $"{UserName(((index + 1) % 30) + 1)}님이 모임에 초대했어요",
                    "support" => $"새로운 '{SupportThemes[(index - 1) % SupportThemes.Length]}' 스토리가 도착했어요",
                    "game" => $"밥게임 결과: {UserName(((index + 3) % 30) + 1)}님이 우승!",
                    _ => "밥메이트 앱 소식",
                };

                var message = type switch
                {
                    "group" => $"{SampleCities[(index - 1) % SampleCities.Length]} 모임을 함께할 친구를 찾고 있어요.",
                    "support" => $"{SupportThemes[(index - 1) % SupportThemes.Length]} 프로젝트를 지금 확인해보세요.",
                    "game" => "방금 종료된 밥게임 결과를 확인하고 축하 메시지를 전달해보세요!",
                    _ => "최신 업데이트와 새로운 기능을 확인해보세요.",
                };

                return new Notification
                {
                    Id = NotificationId(index),
                    Type = type,
                    Title = title,
                    Message = message,
                    Time = DateTimeIsoString(index / 2, 8 + (index % 4)),
                };
            })
            .ToArray();

    private static IReadOnlyCollection<ChatThread> CreateChatThreads() =>
        Enumerable.Range(1, 30)
            .Select(index =>
            {
                var hostIndex = ((index - 1) % 10) + 1;
                var participantIndex = ((index + 4) % 30) + 1;

                var messages = new[]
                {
                    new
                    {
                        id = $"msg-{index:000}-1",
                        senderId = UserId(hostIndex),
                        senderName = UserName(hostIndex),
                        content = $"{UserName(hostIndex)}님이 모임을 시작했어요.",
                        timestamp = DateTimeIsoString(index, 12),
                    },
                    new
                    {
                        id = $"msg-{index:000}-2",
                        senderId = UserId(participantIndex),
                        senderName = UserName(participantIndex),
                        content = "참여 확인했습니다! 장소가 기대돼요.",
                        timestamp = DateTimeIsoString(index, 13),
                    },
                };

                return new ChatThread
                {
                    Id = ChatId(index),
                    GroupId = GroupId(index),
                    Title = $"{SampleCities[(index - 1) % SampleCities.Length]} 모임 톡 {index:D2}",
                    MessagesJson = Json(messages),
                };
            })
            .ToArray();

    private static IReadOnlyCollection<GameHistory> CreateGameHistories() =>
        Enumerable.Range(1, 30)
            .Select(index =>
            {
                var gameTypes = new[] { "roulette", "ladder", "ranking", "rps", "dice" };
                var participants = new[]
                {
                    UserName(((index - 1) % 30) + 1),
                    UserName(((index + 1) % 30) + 1),
                    UserName(((index + 5) % 30) + 1),
                };

                var winner = participants[(index + 1) % participants.Length];
                var tags = new[] { "친구", "정산", "미션", "번개", "밥게임" };
                var tagSlice = tags
                    .Skip(index % tags.Length)
                    .Concat(tags)
                    .Take(2)
                    .ToArray();

                return new GameHistory
                {
                    Id = GameId(index),
                    Type = gameTypes[(index - 1) % gameTypes.Length],
                    CreatedAt = DateTimeIsoString(index, 18),
                    ParticipantsJson = Json(participants),
                    Winner = winner,
                    Mode = index % 2 == 0 ? "open" : "direct",
                    MaxParticipants = 6,
                    Description = $"{UserName(((index - 1) % 30) + 1)}님의 밥게임 {index:D2}",
                    TagsJson = Json(tagSlice),
                    Wager = index % 3 == 0 ? Json(new[] { "밥값 전액", "음료 쏘기", "후기 작성" }) : null,
                    Status = index % 3 == 0 ? "completed" : "scheduled",
                    HostId = UserId(((index - 1) % 10) + 1),
                    GameMode = index % 2 == 0 ? "individual" : "team",
                    GameResultJson = Json(new
                    {
                        type = gameTypes[(index - 1) % gameTypes.Length],
                        winner,
                        participants,
                        timestamp = DateTimeIsoString(index, 20),
                        details = new { note = "샘플 데이터로 생성된 결과입니다." },
                    }),
                    GroupId = index % 3 == 0 ? GroupId(((index - 1) % 6) + 1) : null,
                    GroupTitle = index % 3 == 0 ? $"샘플 모임 {((index - 1) % 6) + 1:D2}" : null,
                };
            })
            .ToArray();

    private static IReadOnlyCollection<Festival> CreateFestivals() =>
        Enumerable.Range(1, 30)
            .Select(index =>
            {
                var city = SampleCities[(index - 1) % SampleCities.Length];
                var district = SampleDistricts[(index - 1) % SampleDistricts.Length];
                var tags = new[] { "푸드트럭", "라이브", "체험", "지역축제", "야시장" };
                var tagSlice = tags
                    .Skip(index % tags.Length)
                    .Concat(tags)
                    .Take(3)
                    .ToArray();

                return new Festival
                {
                    Id = FestivalId(index),
                    Title = $"{city} 미식 축제 {index:D2}",
                    Description = $"{city} {district}에서 열리는 지역 미식 축제로 다양한 먹거리와 공연이 함께해요.",
                    Location = $"{city} {district} 문화광장",
                    Address = $"{city} {district} 맛나로 {20 + index}",
                    Latitude = 36.8 + (index * 0.02),
                    Longitude = 127.2 + (index * 0.015),
                    StartDate = DateOnlyString(index * 2),
                    EndDate = DateOnlyString(index * 2 + 2),
                    StartTime = "11:00",
                    EndTime = "22:00",
                    Category = FestivalCategories[(index - 1) % FestivalCategories.Length],
                    Organizer = FestivalOrganizers[(index - 1) % FestivalOrganizers.Length],
                    ContactInfo = $"02-7{index:000}-{4000 + index}",
                    ImagesJson = Json(new[] { $"https://picsum.photos/seed/festival{index}/800/450" }),
                    TagsJson = Json(tagSlice),
                    ParticipantCount = 40 + (index * 3),
                    MaxParticipants = 200 + (index * 6),
                    Likes = 12 + (index * 2),
                    IsLiked = index % 4 == 0,
                    TicketPrice = index % 5 == 0 ? 5000 + (index * 200) : null,
                    IsBookingRequired = index % 3 == 0,
                    BookingUrl = index % 3 == 0 ? $"https://bapmate.app/festivals/{index:D2}/booking" : null,
                    FacilitiesJson = Json(new[] { "주차장", "포토존", "휴게존" }),
                    AccessibilityJson = Json(new[] { "휠체어 출입 가능", "유아 휴게실" }),
                    Parking = index % 2 == 0,
                    PublicTransportJson = Json(new[] { "지하철 2호선", "버스 700번" }),
                    Website = $"https://bapmate.app/festivals/{index:D2}",
                    SocialMediaJson = Json(new { instagram = $"https://instagram.com/bapmate.festival{index:D2}" }),
                    ContentJson = Json(new
                    {
                        highlights = new[] { "지역 상인과 함께하는 먹거리 부스", "공연과 체험 프로그램" },
                        schedule = new[]
                        {
                            new { time = "12:00", title = "오프닝 푸드 퍼레이드" },
                            new { time = "18:00", title = "라이브 공연" },
                        },
                    }),
                    WeatherJson = Json(new { forecast = "맑음", temperature = 22 + (index % 6) }),
                    NearbyRestaurantsJson = Json(new[] { RestaurantId(((index - 1) % 30) + 1) }),
                    ReviewsJson = Json(new[]
                    {
                        new
                        {
                            id = $"festival-review-{index:000}",
                            userName = UserName(((index + 2) % 30) + 1),
                            rating = 4 + (index % 2),
                            comment = "분위기가 좋고 먹을거리가 다양했어요.",
                            date = DateOnlyString(index * 2 + 3),
                        },
                    }),
                };
            })
            .ToArray();

    private static IReadOnlyCollection<MatchRequest> CreateMatchRequests() =>
        Enumerable.Range(1, 30)
            .Select(index =>
            {
                var city = SampleCities[(index - 1) % SampleCities.Length];
                var genderPreference = index % 3 == 0 ? "female" : index % 5 == 0 ? "male" : "any";
                var paymentType = index % 4 == 0 ? "host-pay" : index % 3 == 0 ? "roulette" : "split";

                var interests = Interests
                    .Skip(index % Interests.Length)
                    .Concat(Interests)
                    .Take(2)
                    .ToArray();

                var suggestions = index % 2 == 0
                    ? Json(new[]
                    {
                        new
                        {
                            id = $"suggest-{index:000}",
                            authorId = UserId(((index + 3) % 30) + 1),
                            authorName = UserName(((index + 3) % 30) + 1),
                            message = $"{city} {CuisineCategories[(index - 1) % CuisineCategories.Length]} 맛집 추천드려요!",
                            restaurantId = RestaurantId(((index - 1) % 30) + 1),
                            createdAt = DateTimeIsoString(index, 11),
                        },
                    })
                    : Json(Array.Empty<object>());

                return new MatchRequest
                {
                    Id = MatchRequestId(index),
                    Title = $"{city} 점심 메이트 찾기 {index:D2}",
                    Message = $"{city}에서 {CuisineCategories[(index - 1) % CuisineCategories.Length]}을(를) 함께 즐길 파트너를 찾습니다.",
                    Location = $"{city} {SampleDistricts[(index - 1) % SampleDistricts.Length]}",
                    RadiusKm = 1.5 + (index % 4) * 0.5,
                    TimeSlot = TimeSlots[(index + 1) % TimeSlots.Length],
                    PreferredAt = DateTimeIsoString(index + 1, 12),
                    PartySize = 2 + (index % 3),
                    GenderPreference = genderPreference,
                    AgeRange = index % 4 == 0 ? "25-34" : null,
                    InterestsJson = Json(interests),
                    DepositRequired = index % 4 == 0,
                    Status = index % 5 == 0 ? "matched" : "open",
                    CreatedAt = DateTimeIsoString(index, 10),
                    CreatedBy = UserId(((index - 1) % 10) + 1),
                    PaymentType = paymentType,
                    ConfirmedWith = index % 5 == 0 ? UserId(((index + 6) % 30) + 1) : null,
                    Notes = index % 3 == 0 ? "맛집 추천 환영합니다!" : null,
                    RestaurantId = index % 3 == 0 ? RestaurantReferenceId(((index - 1) % 30) + 1) : null,
                    AllowRestaurantSuggestions = true,
                    SuggestionsJson = suggestions,
                };
            })
            .ToArray();

    private static IReadOnlyCollection<UserDirectoryEntry> CreateUserDirectoryEntries() =>
        Enumerable.Range(1, 30)
            .Select(static index => new UserDirectoryEntry
            {
                Id = UserId(index),
                Name = UserName(index),
                Avatar = AvatarFor(index),
                Phone = PhoneFor(index),
                Identifier = $"bap{index:D2}",
                Bio = $"{SampleCities[(index - 1) % SampleCities.Length]}에서 활동 중인 밥친구입니다.",
            })
            .ToArray();

    private static IReadOnlyCollection<RestaurantReference> CreateRestaurantDirectory() =>
        Enumerable.Range(1, 30)
            .Select(index =>
            {
                var city = SampleCities[(index - 1) % SampleCities.Length];
                var category = CuisineCategories[(index - 1) % CuisineCategories.Length];

                return new RestaurantReference
                {
                    Id = RestaurantReferenceId(index),
                    Name = $"{city} {category} 스팟 {index:D2}",
                    Address = $"{city} {SampleDistricts[(index - 1) % SampleDistricts.Length]} 맛길 {100 + index}",
                    Category = category,
                    City = city,
                    TagsJson = Json(new[]
                    {
                        category,
                        "지역맛집",
                        index % 2 == 0 ? "단골추천" : "데이트",
                    }),
                };
            })
            .ToArray();

    private static string UserId(int index) => $"user-{index:00}";
    private static string FriendId(int index) => $"friend-{index:00}";
    private static string SupportId(int index) => $"support-{index:00}";
    private static string GroupId(int index) => $"group-{index:00}";
    private static string RestaurantId(int index) => $"restaurant-{index:00}";
    private static string NotificationId(int index) => $"notification-{index:00}";
    private static string ChatId(int index) => $"chat-{index:00}";
    private static string GameId(int index) => $"game-{index:00}";
    private static string FestivalId(int index) => $"festival-{index:00}";
    private static string MatchRequestId(int index) => $"match-{index:00}";
    private static string RestaurantReferenceId(int index) => $"ext-{index:000}";
    private static string PaymentTransactionId(int userIndex, int sequence) => $"pay-{userIndex:00}-{sequence:000}";

    private static string UserName(int index) => $"샘플 메이트 {index:00}";

    private static string AvatarFor(int index) => $"https://i.pravatar.cc/150?img={((index - 1) % 70) + 1}";

    private static string PhoneFor(int index) => $"010-{1000 + index:0000}";

    private static string DateOnlyString(int dayOffset) =>
        BaseDate.AddDays(dayOffset).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static string DateTimeIsoString(int dayOffset, int hourOffset) =>
        BaseDate.AddDays(dayOffset).AddHours(hourOffset).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

    private static string DateTimeLocalString(int dayOffset, int hour, int minute) =>
        BaseDate.AddDays(dayOffset).AddHours(hour).AddMinutes(minute).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

    private static string Json<T>(T value) =>
        JsonSerializer.Serialize(value, SerializerOptions);
}
