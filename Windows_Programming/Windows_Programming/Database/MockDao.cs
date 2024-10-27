using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Model;
using Windows_Programming.Database;
using System.Reflection.Metadata;
using System.Security.Principal;

namespace Windows_Programming.Database
{
    public class MockDao : IDao
    {
        public List<Plan> GetAllPlanInTrashCan()
        {
            var result = new List<Plan>
                {
                    new Plan
                    {
                        Name = "Plan 1",
                        PlanImage = "/Assets/danang.jpg",
                        StartDate = new DateTime(2023, 1, 1),
                        EndDate = new DateTime(2023, 1, 10),
                        Description = "Description for Plan 1",
                        StartLocation = "Location A",
                        EndLocation = "Location B",
                        Activities = new List<Activity>
                        {
                            new Activity { Name = "Activity 1", Description = "Description for Activity 1" },
                            new Activity { Name = "Activity 2", Description = "Description for Activity 2" }
                        },
                        Type = true, // Traveller
                        DeletedDate = new DateTime(2023, 1, 8),
                    },
                    new Plan
                    {
                        Name = "Plan 2",
                        PlanImage = "/Assets/danang.jpg",
                        StartDate = new DateTime(2023, 2, 1),
                        EndDate = new DateTime(2023, 2, 10),
                        Description = "Description for Plan 2",
                        StartLocation = "Location C",
                        EndLocation = "Location D",
                        Activities = new List<Activity>
                        {
                            new Activity { Name = "Activity 3", Description = "Description for Activity 3" },
                            new Activity { Name = "Activity 4", Description = "Description for Activity 4" }
                        },
                        Type = false, // Non-Traveller
                        DeletedDate = new DateTime(2023, 2, 8),
                    },
                    new Plan
                    {
                        Name = "Plan 3",
                        PlanImage = "/Assets/danang.jpg",
                        StartDate = new DateTime(2023, 3, 1),
                        EndDate = new DateTime(2023, 3, 10),
                        Description = "Description for Plan 3",
                        StartLocation = "Location E",
                        EndLocation = "Location F",
                        Activities = new List<Activity>
                        {
                            new Activity { Name = "Activity 5", Description = "Description for Activity 5" },
                            new Activity { Name = "Activity 6", Description = "Description for Activity 6" }
                        },
                        Type = true, // Traveller
                        DeletedDate = new DateTime(2023, 3, 8),
                    },
                    new Plan
                    {
                        Name = "Plan 4",
                        PlanImage = "/Assets/danang.jpg",
                        StartDate = new DateTime(2023, 4, 1),
                        EndDate = new DateTime(2023, 4, 10),
                        Description = "Description for Plan 4",
                        StartLocation = "Location G",
                        EndLocation = "Location H",
                        Activities = new List<Activity>
                        {
                            new Activity { Name = "Activity 7", Description = "Description for Activity 7" },
                            new Activity { Name = "Activity 8", Description = "Description for Activity 8" }
                        },
                        Type = false, // Non-Traveller
                        DeletedDate = new DateTime(2023, 3, 8),
                    },
                    new Plan
                    {
                        Name = "Plan 5",
                        PlanImage = "/Assets/danang.jpg",
                        StartDate = new DateTime(2023, 5, 1),
                        EndDate = new DateTime(2023, 5, 10),
                        Description = "Description for Plan 5",
                        StartLocation = "Location I",
                        EndLocation = "Location J",
                        Activities = new List<Activity>
                        {
                            new Activity { Name = "Activity 9", Description = "Description for Activity 9" },
                            new Activity { Name = "Activity 10", Description = "Description for Activity 10" }
                        },
                        Type = true, // Traveller
                        DeletedDate = new DateTime(2023, 5, 8),
                    }
                };
            return result;
        }

        public List<Account> GetAllAccount()
        {
            var result = new List<Account>
                {
                    new Account
                    {
                        Username = "admin",
                        Email = "abd",
                        Address ="xit",
                        Fullname ="haha"
                    },
                    new Account
                    {
                        Username = "dog",
                        Email = "bird",
                        Address ="fish",
                        Fullname ="mouse"
                    }
                };
            return result;
        }


        public List<Tour> GetAllTour()
        {
            var result = new List<Tour>
                {
                    new Tour
                    {
                        Id = 1,
                        Name = "Test",
                        Description = "Test",
                        Image = "ms-appx:///Assets/sample-image.jpg",
                        Rating = 4,
                        Places = new List<string> { "ho chi minh", "Da nang" },
                        Price = 1
                    }
                };
            return result;
        }

        public List<Blog> GetAllBlog()
        {
            // tạo 1 danh sách blog hoàn chỉnh 
            var result = new List<Blog>
                {
                    new Blog
                    {
                        Id = 1,
                        Title = "Nhà sách Hải An - Địa điểm “sống ảo” cực chill tại Sài Gòn",
                        Content = "Không chỉ là một địa điểm check-in, sống ảo cực kỳ nghệ thuật, nhà sách Hải An còn là địa điểm văn hóa giáo dục nổi tiếng, " +
                        "mang đến cho đọc giả tại một không gian tràn ngập tri thức. Nhờ những ưu điểm ấy đã giúp nhà sách Hải An trở thành điểm đến lý thú cho " +
                        "cả người dân địa phương lẫn du khách khi du lịch đến thành phố mang tên Bác. Để có cái nhìn rõ ràng hơn về những điều thú vị tại nhà sách " +
                        "Hải An thì bạn đừng bỏ lỡ bài viết bên dưới của Traveloka nhé!\n" +
                        "Giới thiệu nhà sách Hải An\r\nNếu ở Hà Nội, mọi người vừa có thể tận hưởng những cuốn sách hay vừa check-in “sống ảo” tại Phố Sách thì tại" +
                        " Thành Phố Hồ Chí Minh, nhà sách Hải An sẽ là nơi lý tưởng để làm những hoạt động ấy. Mặc dù được gọi là nhà sách, nhưng nhà sách Hải An không " +
                        "chỉ đơn thuần là địa điểm để mua sách và dụng cụ học tập, mà nơi đây còn tinh tế biến chủ đề biển cả trở thành lối thiết kế chính của tòa nhà." +
                        " Từ đó, nhà sách Hải An trở thành một địa điểm check-in cực kỳ nghệ thuật dành cho mọi lứa tuổi ở Sài Gòn. Nhờ đó mà nhà sách Hải An được nhiều " +
                        "bạn trẻ đặt cho biệt danh “Atlantis”. Ngụ ý rằng nơi đây như là một đại dương tri thức dành cho tất cả mọi người.",
                        Author = "Nguyễn Thụy Mộc Nhiên",
                        PublishDate = new DateTime(2024,11,24),
                        Image = "ms-appx:///Assets/Blog/blog01.jpg"
                    },
                    new Blog
                    {
                        Id = 2,
                        Title = "Bánh tráng cuốn thịt heo - Món ăn đặc biệt từ Đà Nẵng",
                        Content = "Một chuyến du lịch Đà Nẵng trọn vẹn là một chuyến đi mà du khách đã thưởng thức món ăn dân dã mang tên bánh tráng cuốn thịt heo." +
                        " Đây là một món ăn truyền thống của Việt Nam ta và nổi tiếng với hương vị cuốn hút cũng như cách chế biến đơn giản. Vậy hiện nay du khách " +
                        "có thể thưởng thức món bánh tráng cuốn thịt heo tại những hàng quán nào tại Việt Nam? Hãy xem qua bài viết bên dưới của Traveloka nhé!\r\n\r\n" +
                        "Giới thiệu về bánh tráng cuốn thịt heo\r\n" +
                        "Bánh tráng cuốn thịt heo từ lâu đã được xem là món ngon tại xứ Quảng nói chung và tại Đà Nẵng nói riêng. Theo thời gian, sự hòa huyện của thịt " +
                        "ba chỉ, bánh tráng, rau thơm,... trong món bánh tráng cuốn thịt heo đã được mọi miền của tổ quốc biết đến.\r\n\r\n" +
                        "banh trang cuon thit heo\r\nMón bánh tráng cuốn thịt heo là đặc sản của xứ Quảng nói chung và Đà Nẵng nói riêng.@Shutterstock\r\n\r\n" +
                        "Nguyên liệu làm bánh tráng cuốn thịt heo\r\n" +
                        "Nguyên liệu tiên quyết của món ăn này chính là những thớ thịt ba chỉ béo ngậy. Ngoài ra, tùy vào từng vùng miền mà sẽ có thêm những nguyên" +
                        " liệu khác nhau. Nhưng một số nguyên liệu phổ biến nhất để tạo nên món ăn này có thể kể đến như:\r\n\r\n" +
                        "Bánh tráng phơi sương\r\nRau thơm\r\nXà lách\r\nXoài xanh\r\nTía tô\r\nChuối xanh\r\nKhế chua\r\nDứa\r\nMắm nêm\r\nỚt\r\n" +
                        "Cách làm bánh tráng cuốn thịt heo\r\nĐể tạo ra một món bánh tráng cuốn thịt heo “chuẩn gu” khá đơn giản. " +
                        "Bạn chỉ cần làm theo những bước sau:\r\n\r\nBước 1: Rửa sạch thịt heo và cho vào nồi luộc với một chút muối trên lửa nhỏ. Khi thịt mềm, " +
                        "vớt ra và cắt lát vừa ăn. Hãy đảm bảo rằng thịt được cắt ra có nạc và mỡ xen kẽ nhé\r\nBước 2: Đổ mắm nêm ra chén và cho thêm tỏi, ớt, dứa " +
                        "cùng 2 muỗng cà phê nước cốt chanh và 2 muỗng cà phê đường. Và sau đó trộn đều hỗn hợp này với nhau\r\nBước 3: Rửa sạch rau củ quả ăn kèm." +
                        " Bắt đầu bày biện các thành phần lên từng đĩa riêng\r\nBước 4: Bắt đầu cuốn thịt cũng như rau vào bánh tráng và thưởng thức món ăn\r\nbanh" +
                        " trang cuon thit heo\r\nĐây là món ăn giải nhiệt thanh đạm với nhiều rau tươi xanh.@Shutterstock\r\n\r\nCác địa điểm nổi tiếng để thưởng thức" +
                        " bánh tráng cuốn thịt heo\r\nBên cạnh việc tự tay tạo nên món bánh tráng cuốn thịt heo thơm ngon, thì bạn cũng có thể thưởng thức nó tại một " +
                        "số hàng quán nổi tiếng. Có thể kể đến như:\r\n\r\nCác quán bán bánh tráng cuốn thịt heo ở Hà Nội\r\nLiệu món ăn “quốc hồn quốc túy” của dân" +
                        " tộc Việt Nam sẽ có hương vị như thế nào tại mảnh đất nghìn năm văn hiến? Hãy đặt vé máy đi Hà Nội và trải nghiệm cũng như tìm câu trả lời tại" +
                        " những quán ăn này cùng Traveloka nhé:\r\n\r\n1. Cò Quán\r\n\r\nĐịa chỉ: 107 đường Hoàng Cầu, Đống Đa, Hà Nội\r\nGiá bán tham khảo:" +
                        " 20,000 - 350,000 VND\r\nNhờ vào vị trí tọa lạc ở mặt tiền đường, du khách sẽ không gặp quá nhiều khó khăn khi tìm đến Cò Quán để thưởng " +
                        "thức bánh tráng cuốn thịt heo. Một suất tại đây không chỉ có đa dạng các loại rau củ quả mà du khách còn có thể tùy chọn loại nhân khác nhau" +
                        ". Từ thịt luộc, thịt quay, nem nướng hay thậm chí là bắp giò luộc. Không chỉ đa dạng mà độ lớn của những miếng thịt tại quán cũng là điều giúp" +
                        " quán nhận về nhiều lời khen có cánh.\r\n\r\nbanh trang cuon thit heo\r\nCò Quán đem tới cho thực khách đa dạng các loại nhân khác nhau." +
                        "@Facebook CÒ QUÁN - 107 Hoàng Cầu - MÓN NGON ĐÀ NẴNG\r\n\r\n2. Đà Nẵng Quán\r\n\r\nĐịa chỉ: 79 đường Trung Hòa, quận Cầu Giấy, Hà Nội\r\nGiá bán tham khảo:" +
                        " 40,000 - 100,000 VND\r\nNhư tên gọi của mình, Đà Nẵng Quán sẽ đem đến cho du khách những món ăn trứ danh từ vùng đất của những cây cầu. Trong số các món" +
                        " ăn ấy thì không thể nào không nhắc đến bánh tráng cuốn thịt heo. Tại đây, du khách có thể tự do chọn giữa thịt luộc ngon ngọt hoặc thịt quay giòn rụm. " +
                        "Ngoài ra, mắm nêm tại quán cũng được làm theo hương vị riêng biệt. Từ đó đem đến trải nghiệm mới lạ cho món bánh tráng cuốn thịt heo.\r\n\r\n",
                        Author = "Nguyễn Thụy Mộc Nhiên",
                        PublishDate = new DateTime(2024,11,24),
                        Image = "ms-appx:///Assets/Blog/blog02.jpg"
                    },
                    new Blog
                    {
                        Id = 3,
                        Title = "Băng rừng tìm đại dương mây Lùng Cúng Yên Bái",
                        Content = "Tôi muốn kể cho bạn nghe về một cuộc hành trình tìm đỉnh cao Lùng Cúng với rất nhiều điều bất ngờ. " +
                        "Là đại dương mây, cánh rừng già, khu rừng tre trúc, những mỏm đá nhẵn thín ru kẻ lữ hành trọn một giấc trưa. " +
                        "Là dải ngân hà huyền ảo trong đêm ở cánh rừng bên con suối, với lều trại, bếp lửa bập bùng thịt nướng. Và cả chú " +
                        "chó tên Tun - người bạn đồng hành ấn tượng, thích đi rừng, vượt dốc, luôn bảo vệ và theo chân chúng tôi trong suốt" +
                        " chuyến đi.\r\n\r\n\r\nChúng tôi quyết định chinh phục ngọn núi Lùng Cúng vào mùa đông năm ngoái. Đây là một đỉnh núi " +
                        "nằm trong top những ngọn núi đẹp nhất ở Việt Nam (cao 2925 m), được đặt tên theo một bản làng nằm sâu trong vùng núi" +
                        " hiểm trở bậc nhất tại xã Nậm Có, huyện Mù Cang Chải, tỉnh Yên Bái. Chuyến đi ấn tượng với bốn người bạn đồng hành " +
                        "cùng một chú chó tên Tun. Hành trình khá gian nan với đoạn đường đá gồ ghề, khó đi và thời tiết mưa lạnh, nhiều sương" +
                        " mù.\r\n\r\nNhưng bù lại, chúng tôi đã có một chuyến leo núi nhiều cảm xúc: tình bạn, tình yêu, sự vượt lên chính mình" +
                        " và hạnh phúc ngập tràn trước thiên nhiên hùng vĩ. Từ Hà Nội chạy xe lên thị trấn Tú Lệ, Yên Bái, chúng tôi nghỉ ngơi" +
                        " để lấy sức cho chặng đường khó nhằn ngày mai. Nghe những người dân nơi đây nói, đường vào bản Tu San rất khó đi." +
                        " Đường bùn đất, lầy lội, dốc cao lại nhỏ hẹp. Gặp thời tiết mưa gió sẽ vô cùng khó khăn để vượt qua chặng đường này. " +
                        "Lo lắng nhiều, nhưng chúng tôi vẫn quyết tâm tìm đường tiếp cận bản làng gần chân núi nhất để trekking chinh phục " +
                        "ngọn núi đẹp này.\r\n\r\nBăng rừng chinh phục đỉnh Lùng Cúng - Mù Căng Chải - Traveloka Golocal\r\nCảnh rừng sáng " +
                        "sớm thật tuyệt với những tia nắng tinh khôi chiếu rọi. Nhâm nhi cốc cà phê giữa cánh rừng cổ thụ, thật tuyệt phải" +
                        " không?\r\n\r\nĐúng như lời những người dân đã nhắc nhở, chúng tôi phải vượt con đường rất gian nan. Ngã xe vì " +
                        "sình lầy, đẩy xe vì dốc đứng. Đồ đạc, áo quần ướt nhẹp và lấm lem bùn đất vì đường nhão nhoét và trơn trượt. " +
                        "Con đường lại dài và ít người dân sinh sống. May mắn thay, chúng tôi đã gặp được một người sống gần chân núi " +
                        "tên là Hờ A Nhà. Anh chàng này không biết nhiều tiếng Kinh nên cũng khiến cuộc trao đổi về thông tin ngọn núi " +
                        "diễn ra khá khó khăn. Thế rồi, sau một hồi thuyết phục, A Nhà đã đồng ý làm người dẫn đường, cùng đoàn chúng tôi " +
                        "lên núi Lùng Cúng.\r\n\r\nBăng rừng chinh phục đỉnh Lùng Cúng - Mù Căng Chải - Traveloka Golocal\r\nĐường vào bản " +
                        "Tu San đầy bùn đất khiến chúng tôi mất cả buổi sáng mới đến được chân núi.\r\n\r\nVượt qua đoạn đường hiểm hóc ấy," +
                        " rời khỏi bản Tu San nằm im lìm trong sương mù đặc quánh, chúng tôi đã lạc bước vào cánh đồng lau bạt ngàn, phấp" +
                        " phới trong gió. Phía xa, lòng chảo đựng đầy mây đang dâng ùn ùn trong buổi chiều sắp tắt nắng. Qua hết những" +
                        " triền lau trắng nhuộm hồng ánh hoàng hôn, chúng tôi bước vào cánh rừng khi ngày sắp tàn. Cây cối rợp che kín " +
                        "cả bầu trời, khiến cho cánh rừng như sập tối.\r\n\r\nBăng rừng chinh phục đỉnh Lùng Cúng - Mù Căng Chải -" +
                        " Traveloka Golocal\r\nRời bản Tu San, chúng tôi bắt gặp những đồi lau bạt ngàn trong gió.\r\n\r\nMột ngày " +
                        "chạy xe khá dài khiến ai nấy đều mệt mỏi và muốn nghỉ chân. Anh A Nhà đưa chúng tôi men đường rừng một đoạn " +
                        "rồi quyết định dừng chân tại một điểm bằng phẳng gần suối. Ai nấy mở đồ, dựng trại, nhóm lửa rồi sửa soạn " +
                        "đồ ăn để nấu nướng. Đó cũng là lúc màn đêm tối đen, bao kín cả khu rừng già. Những bữa ăn trong rừng già lúc" +
                        " nào cũng thật ấn tượng. Mỗi người một việc, người trải nệm lá dưới lều nằm cho êm, người rán bánh chưng, " +
                        "người xiên thịt nướng, người đi tìm củi. Bữa tối phút chốc cũng hoàn thành trong cái lành lạnh của rừng " +
                        "đêm xuống.\r\n\r\nBăng rừng chinh phục đỉnh Lùng Cúng - Mù Căng Chải - Traveloka Golocal\r\nĐêm trong rừng, " +
                        "chúng tôi chia nhau dựng trại, hạ lều, nhóm lửa và xiên thịt nướng. Một trài nghiệm thú vị bên những người bạn" +
                        " đồng hành.\r\n\r\nRồi trong cái bập bùng của lửa đêm, tiếng suối róc rách và tiếng côn trùng kêu, chúng tôi " +
                        "ngồi cạnh bên nhau, kể lể, chuyện trò những câu chuyện cho đêm bớt lạnh. Đêm im lìm, ngửa mặt nhìn trời và tận" +
                        " hưởng vẻ đẹp dải ngân hà lấp lánh, thật sự là một cảm xúc khó diễn tả. Vòm trời như gần hơn, ngỡ như chúng " +
                        "tôi đang ở một thế giới khác, xa hẳn những chật chội, ồn ào phố thị ngoài kia. A Hờ chuẩn bị sẵn củi lửa để " +
                        "sưởi ấm qua đêm nay. Trời càng về khuya càng lạnh. Chúng tôi chui vào túi ngủ, đội mũ, quàng khăn kín mít rồi " +
                        "vào lều nằm. Ai đó còn mang theo chiếc loa nhỏ, bật vài bài hát đã cũ. Giấc ngủ trôi vào đêm. Mộng mị.\r\n\r\nCảm " +
                        "giác thật trong lành khi buổi sớm thức dậy với hơi mát của sương đọng, với tia nắng chiếu xiên những hạt bụi rừng" +
                        " li ti và mọi thứ đều xanh. Thu xếp đồ đạc, cả đoàn lại lên đường với mục tiêu giữa buổi trưa sẽ lên đến đỉnh Lùng" +
                        " Cúng rồi trở về lại bản Tu San trước khi đêm xuống.\r\n\r\nCảnh bình minh trong khu rừng thật tuyệt. Dường như " +
                        "chúng tôi đang đi xuyên khu rừng nguyên sinh có cảnh quan đẹp nhất vùng Tu San này. Những phiến đá to lớn, " +
                        "rêu phong, bằng phẳng. Suối thác đan xen rừng rậm. Đi xa hơn chút lại bắt gặp những thảm thực vật độc đáo với những" +
                        " tán trúc đan xen nhau trong bóng nắng như khung cảnh của bộ phim \"Thập diện mai phục\". Càng lên cao, càng nhiều " +
                        "những cây sặc lá nhỏ, thân nhỏ hơn thân trúc, đan nhau chằng chịt.\r\n\r\nBăng rừng chinh phục đỉnh Lùng Cúng - " +
                        "Mù Căng Chải - Traveloka Golocal\r\nTrekking trong khu rừng tre trúc đẹp mê hoặc như khung cảnh của bộ phim \"" +
                        "Thập diện mai phục\".\r\n\r\nA Nhà chỉ cho chúng tôi thấy những cái bẫy chuột làm bằng thân tre, trúc. Rất khó để " +
                        "nhận ra những cái bẫy nguy hiểm này vì nó lẫn với màu nâu, màu xanh của trúc, của sặc. A Nhà không an tâm nên kiếm " +
                        "một sợi dây leo buộc cổ chú chó Tun lại để dắt theo. Tun chạy rất nhanh, dường như đã quen đường rừng trong mỗi " +
                        "chuyến đi cùng chủ nên khá dạn dĩ, không sợ điều gì phía trước.\r\n\r\nCũng bởi cái tinh nhạy của loài chó núi nên " +
                        "một vài lúc tưởng như nó đã lạc mất đoàn vì gọi mãi không thấy bóng dáng đâu, vậy mà lát sau đã thấy nó đứng phía " +
                        "trước đợi cả đoàn. Thuộc đường, cẩn trọng với bẫy, không sợ rừng sâu, quyết tâm đi theo chủ lên tới đỉnh. Đó có lẽ" +
                        " là điều khiến chúng tôi ấn tượng với chú chó núi này.\r\n\r\nBăng rừng chinh phục đỉnh Lùng Cúng - Mù Căng Chải -" +
                        " Traveloka Golocal\r\nTun – chú chó núi nhanh nhẹn luôn đi trước tìm đường hoặc quay lại phía sau tìm thành viên" +
                        " trong đoàn nếu có ai đi chậm bị bỏ lại\r\n\r\nGần đỉnh núi là những triền đồi hoa trắng với cái gió của vực sâu " +
                        "như hút xuống. Trời trưa vẫn có nắng, mà lạnh buốt vì gió trời. Vượt những thảm hoa trắng, chúng tôi dừng lại bên" +
                        " lưng chừng đồi gió để xác định phương hướng. Có lẽ đỉnh phía xa kia chính là đỉnh Lùng Cúng mà chúng tôi kiếm tìm. " +
                        "Gió càng lúc càng mạnh. Bước chân trên những thảm cỏ vàng, cỏ nâu, ngọn đồi tưởng thoải mà lại chấp chới vì gió từ " +
                        "dưới thung thổi lên quá mạnh.\r\n\r\nAi nấy chỉ biết bám chặt tay vào những thân cây cỏ dai nơi triền đồi mà đạp " +
                        "ngược chân lên bước. Trời càng lúc càng xanh. Mây cuộn và trôi nhanh như hình ảnh tôi thường nhìn qua video timelapse " +
                        "(đoạn clip tua nhanh). Những khoảnh khắc kì lạ này của thiên nhiên càng thôi thúc dấu chân chúng tôi thêm mạnh bước" +
                        ".\r\n\r\nBăng rừng chinh phục đỉnh Lùng Cúng - Mù Căng Chải - Traveloka Golocal\r\nTriền đồi gần đỉnh với rất nhiều " +
                        "gió và biển mây cuộn sóng\r\n\r\nChúng tôi đi men những đoạn vực gió lớn, với những triền hoa trắng nhuộm nắng thảo " +
                        "nguyên. Bốn bề là núi, với những bản làng nhỏ, xa tít bên kia thung lũng. Và rồi, một cảm giác hạnh phúc len lỏi " +
                        "trong tim khi nghe tiếng người bạn đồng hành từ xa hô lên: “Đỉnh đây rồi, lên đến đỉnh Lùng Cúng rồi!” Cái hồi hộp," +
                        " rạo rực cứ dấy lên khiến người ta xúc động tột cùng. Dù gió đang hút xuống sau lưng, đang thốc ngược vào mặt, nhưng" +
                        " vẫn bước thật nhanh để vượt nốt con dốc này thôi là đặt chân đến đỉnh cao rồi. Đuôi mắt cay cay khi nhìn thấy ngọn " +
                        "cờ đỏ chót mấy người bạn lên trước vừa dựng lên.\r\n\r\nBăng rừng chinh phục đỉnh Lùng Cúng - Mù Căng Chải - " +
                        "Traveloka Golocal\r\nChúng tôi lạc bước vào đồi cỏ gió trên đại dương mây. Tôi muốn nằm đó, nơi gió và mây đang " +
                        "vờn đuổi. Đánh trọn một giấc say, hít no nê mùi cỏ vàng.\r\n\r\nĐỉnh Lùng Cúng có góc nhìn thoáng và rất rộng. " +
                        "Cảm giác như có cả một sân bóng trên đỉnh núi vậy. Xa xa, biển mây trôi trắng đang che bản Lùng Cúng. Khung cảnh " +
                        "quá thần tiên. Chỉ biết rằng giây phút đó, mỗi người một góc, đứng lặng lại trước món quà quý giá mà ngọn núi đã " +
                        "dành tặng cho những kẻ chịu khó vượt xa xôi, vượt gian nan để đến được nơi này.\r\n\r\n",
                        Author = "Nguyễn Hạnh Hà My",
                        PublishDate = new DateTime(2021,09,21),
                        Image = "ms-appx:///Assets/Blog/blog03.jpg"
                    },
                    new Blog
                    {
                        Id = 4,
                        Title = "Bánh tráng cuốn thịt heo - Món ăn đặc biệt từ Đà Nẵng",
                        Content = "Một chuyến du lịch Đà Nẵng trọn vẹn là một chuyến đi mà du khách đã thưởng thức món ăn dân dã mang tên bánh tráng cuốn thịt heo." +
                        " Đây là một món ăn truyền thống của Việt Nam ta và nổi tiếng với hương vị cuốn hút cũng như cách chế biến đơn giản. Vậy hiện nay du khách " +
                        "có thể thưởng thức món bánh tráng cuốn thịt heo tại những hàng quán nào tại Việt Nam? Hãy xem qua bài viết bên dưới của Traveloka nhé!\r\n\r\n" +
                        "Giới thiệu về bánh tráng cuốn thịt heo\r\n" +
                        "Bánh tráng cuốn thịt heo từ lâu đã được xem là món ngon tại xứ Quảng nói chung và tại Đà Nẵng nói riêng. Theo thời gian, sự hòa huyện của thịt " +
                        "ba chỉ, bánh tráng, rau thơm,... trong món bánh tráng cuốn thịt heo đã được mọi miền của tổ quốc biết đến.\r\n\r\n" +
                        "banh trang cuon thit heo\r\nMón bánh tráng cuốn thịt heo là đặc sản của xứ Quảng nói chung và Đà Nẵng nói riêng.@Shutterstock\r\n\r\n" +
                        "Nguyên liệu làm bánh tráng cuốn thịt heo\r\n" +
                        "Nguyên liệu tiên quyết của món ăn này chính là những thớ thịt ba chỉ béo ngậy. Ngoài ra, tùy vào từng vùng miền mà sẽ có thêm những nguyên" +
                        " liệu khác nhau. Nhưng một số nguyên liệu phổ biến nhất để tạo nên món ăn này có thể kể đến như:\r\n\r\n" +
                        "Bánh tráng phơi sương\r\nRau thơm\r\nXà lách\r\nXoài xanh\r\nTía tô\r\nChuối xanh\r\nKhế chua\r\nDứa\r\nMắm nêm\r\nỚt\r\n" +
                        "Cách làm bánh tráng cuốn thịt heo\r\nĐể tạo ra một món bánh tráng cuốn thịt heo “chuẩn gu” khá đơn giản. " +
                        "Bạn chỉ cần làm theo những bước sau:\r\n\r\nBước 1: Rửa sạch thịt heo và cho vào nồi luộc với một chút muối trên lửa nhỏ. Khi thịt mềm, " +
                        "vớt ra và cắt lát vừa ăn. Hãy đảm bảo rằng thịt được cắt ra có nạc và mỡ xen kẽ nhé\r\nBước 2: Đổ mắm nêm ra chén và cho thêm tỏi, ớt, dứa " +
                        "cùng 2 muỗng cà phê nước cốt chanh và 2 muỗng cà phê đường. Và sau đó trộn đều hỗn hợp này với nhau\r\nBước 3: Rửa sạch rau củ quả ăn kèm." +
                        " Bắt đầu bày biện các thành phần lên từng đĩa riêng\r\nBước 4: Bắt đầu cuốn thịt cũng như rau vào bánh tráng và thưởng thức món ăn\r\nbanh" +
                        " trang cuon thit heo\r\nĐây là món ăn giải nhiệt thanh đạm với nhiều rau tươi xanh.@Shutterstock\r\n\r\nCác địa điểm nổi tiếng để thưởng thức" +
                        " bánh tráng cuốn thịt heo\r\nBên cạnh việc tự tay tạo nên món bánh tráng cuốn thịt heo thơm ngon, thì bạn cũng có thể thưởng thức nó tại một " +
                        "số hàng quán nổi tiếng. Có thể kể đến như:\r\n\r\nCác quán bán bánh tráng cuốn thịt heo ở Hà Nội\r\nLiệu món ăn “quốc hồn quốc túy” của dân" +
                        " tộc Việt Nam sẽ có hương vị như thế nào tại mảnh đất nghìn năm văn hiến? Hãy đặt vé máy đi Hà Nội và trải nghiệm cũng như tìm câu trả lời tại" +
                        " những quán ăn này cùng Traveloka nhé:\r\n\r\n1. Cò Quán\r\n\r\nĐịa chỉ: 107 đường Hoàng Cầu, Đống Đa, Hà Nội\r\nGiá bán tham khảo:" +
                        " 20,000 - 350,000 VND\r\nNhờ vào vị trí tọa lạc ở mặt tiền đường, du khách sẽ không gặp quá nhiều khó khăn khi tìm đến Cò Quán để thưởng " +
                        "thức bánh tráng cuốn thịt heo. Một suất tại đây không chỉ có đa dạng các loại rau củ quả mà du khách còn có thể tùy chọn loại nhân khác nhau" +
                        ". Từ thịt luộc, thịt quay, nem nướng hay thậm chí là bắp giò luộc. Không chỉ đa dạng mà độ lớn của những miếng thịt tại quán cũng là điều giúp" +
                        " quán nhận về nhiều lời khen có cánh.\r\n\r\nbanh trang cuon thit heo\r\nCò Quán đem tới cho thực khách đa dạng các loại nhân khác nhau." +
                        "@Facebook CÒ QUÁN - 107 Hoàng Cầu - MÓN NGON ĐÀ NẴNG\r\n\r\n2. Đà Nẵng Quán\r\n\r\nĐịa chỉ: 79 đường Trung Hòa, quận Cầu Giấy, Hà Nội\r\nGiá bán tham khảo:" +
                        " 40,000 - 100,000 VND\r\nNhư tên gọi của mình, Đà Nẵng Quán sẽ đem đến cho du khách những món ăn trứ danh từ vùng đất của những cây cầu. Trong số các món" +
                        " ăn ấy thì không thể nào không nhắc đến bánh tráng cuốn thịt heo. Tại đây, du khách có thể tự do chọn giữa thịt luộc ngon ngọt hoặc thịt quay giòn rụm. " +
                        "Ngoài ra, mắm nêm tại quán cũng được làm theo hương vị riêng biệt. Từ đó đem đến trải nghiệm mới lạ cho món bánh tráng cuốn thịt heo.\r\n\r\n",
                        Author = "Nguyễn Thụy Mộc Nhiên",
                        PublishDate = new DateTime(2024,11,24),
                        Image = "ms-appx:///Assets/Blog/blog02.jpg"
                    },
                    new Blog
                    {
                        Id = 5,
                        Title = "Bánh tráng cuốn thịt heo - Món ăn đặc biệt từ Đà Nẵng",
                        Content = "Một chuyến du lịch Đà Nẵng trọn vẹn là một chuyến đi mà du khách đã thưởng thức món ăn dân dã mang tên bánh tráng cuốn thịt heo." +
                        " Đây là một món ăn truyền thống của Việt Nam ta và nổi tiếng với hương vị cuốn hút cũng như cách chế biến đơn giản. Vậy hiện nay du khách " +
                        "có thể thưởng thức món bánh tráng cuốn thịt heo tại những hàng quán nào tại Việt Nam? Hãy xem qua bài viết bên dưới của Traveloka nhé!\r\n\r\n" +
                        "Giới thiệu về bánh tráng cuốn thịt heo\r\n" +
                        "Bánh tráng cuốn thịt heo từ lâu đã được xem là món ngon tại xứ Quảng nói chung và tại Đà Nẵng nói riêng. Theo thời gian, sự hòa huyện của thịt " +
                        "ba chỉ, bánh tráng, rau thơm,... trong món bánh tráng cuốn thịt heo đã được mọi miền của tổ quốc biết đến.\r\n\r\n" +
                        "banh trang cuon thit heo\r\nMón bánh tráng cuốn thịt heo là đặc sản của xứ Quảng nói chung và Đà Nẵng nói riêng.@Shutterstock\r\n\r\n" +
                        "Nguyên liệu làm bánh tráng cuốn thịt heo\r\n" +
                        "Nguyên liệu tiên quyết của món ăn này chính là những thớ thịt ba chỉ béo ngậy. Ngoài ra, tùy vào từng vùng miền mà sẽ có thêm những nguyên" +
                        " liệu khác nhau. Nhưng một số nguyên liệu phổ biến nhất để tạo nên món ăn này có thể kể đến như:\r\n\r\n" +
                        "Bánh tráng phơi sương\r\nRau thơm\r\nXà lách\r\nXoài xanh\r\nTía tô\r\nChuối xanh\r\nKhế chua\r\nDứa\r\nMắm nêm\r\nỚt\r\n" +
                        "Cách làm bánh tráng cuốn thịt heo\r\nĐể tạo ra một món bánh tráng cuốn thịt heo “chuẩn gu” khá đơn giản. " +
                        "Bạn chỉ cần làm theo những bước sau:\r\n\r\nBước 1: Rửa sạch thịt heo và cho vào nồi luộc với một chút muối trên lửa nhỏ. Khi thịt mềm, " +
                        "vớt ra và cắt lát vừa ăn. Hãy đảm bảo rằng thịt được cắt ra có nạc và mỡ xen kẽ nhé\r\nBước 2: Đổ mắm nêm ra chén và cho thêm tỏi, ớt, dứa " +
                        "cùng 2 muỗng cà phê nước cốt chanh và 2 muỗng cà phê đường. Và sau đó trộn đều hỗn hợp này với nhau\r\nBước 3: Rửa sạch rau củ quả ăn kèm." +
                        " Bắt đầu bày biện các thành phần lên từng đĩa riêng\r\nBước 4: Bắt đầu cuốn thịt cũng như rau vào bánh tráng và thưởng thức món ăn\r\nbanh" +
                        " trang cuon thit heo\r\nĐây là món ăn giải nhiệt thanh đạm với nhiều rau tươi xanh.@Shutterstock\r\n\r\nCác địa điểm nổi tiếng để thưởng thức" +
                        " bánh tráng cuốn thịt heo\r\nBên cạnh việc tự tay tạo nên món bánh tráng cuốn thịt heo thơm ngon, thì bạn cũng có thể thưởng thức nó tại một " +
                        "số hàng quán nổi tiếng. Có thể kể đến như:\r\n\r\nCác quán bán bánh tráng cuốn thịt heo ở Hà Nội\r\nLiệu món ăn “quốc hồn quốc túy” của dân" +
                        " tộc Việt Nam sẽ có hương vị như thế nào tại mảnh đất nghìn năm văn hiến? Hãy đặt vé máy đi Hà Nội và trải nghiệm cũng như tìm câu trả lời tại" +
                        " những quán ăn này cùng Traveloka nhé:\r\n\r\n1. Cò Quán\r\n\r\nĐịa chỉ: 107 đường Hoàng Cầu, Đống Đa, Hà Nội\r\nGiá bán tham khảo:" +
                        " 20,000 - 350,000 VND\r\nNhờ vào vị trí tọa lạc ở mặt tiền đường, du khách sẽ không gặp quá nhiều khó khăn khi tìm đến Cò Quán để thưởng " +
                        "thức bánh tráng cuốn thịt heo. Một suất tại đây không chỉ có đa dạng các loại rau củ quả mà du khách còn có thể tùy chọn loại nhân khác nhau" +
                        ". Từ thịt luộc, thịt quay, nem nướng hay thậm chí là bắp giò luộc. Không chỉ đa dạng mà độ lớn của những miếng thịt tại quán cũng là điều giúp" +
                        " quán nhận về nhiều lời khen có cánh.\r\n\r\nbanh trang cuon thit heo\r\nCò Quán đem tới cho thực khách đa dạng các loại nhân khác nhau." +
                        "@Facebook CÒ QUÁN - 107 Hoàng Cầu - MÓN NGON ĐÀ NẴNG\r\n\r\n2. Đà Nẵng Quán\r\n\r\nĐịa chỉ: 79 đường Trung Hòa, quận Cầu Giấy, Hà Nội\r\nGiá bán tham khảo:" +
                        " 40,000 - 100,000 VND\r\nNhư tên gọi của mình, Đà Nẵng Quán sẽ đem đến cho du khách những món ăn trứ danh từ vùng đất của những cây cầu. Trong số các món" +
                        " ăn ấy thì không thể nào không nhắc đến bánh tráng cuốn thịt heo. Tại đây, du khách có thể tự do chọn giữa thịt luộc ngon ngọt hoặc thịt quay giòn rụm. " +
                        "Ngoài ra, mắm nêm tại quán cũng được làm theo hương vị riêng biệt. Từ đó đem đến trải nghiệm mới lạ cho món bánh tráng cuốn thịt heo.\r\n\r\n",
                        Author = "Nguyễn Thụy Mộc Nhiên",
                        PublishDate = new DateTime(2024,11,24),
                        Image = "ms-appx:///Assets/Blog/blog02.jpg"
                    },
                    new Blog
                    {
                        Id = 6,
                        Title = "Bánh tráng cuốn thịt heo - Món ăn đặc biệt từ Đà Nẵng",
                        Content = "Một chuyến du lịch Đà Nẵng trọn vẹn là một chuyến đi mà du khách đã thưởng thức món ăn dân dã mang tên bánh tráng cuốn thịt heo." +
                        " Đây là một món ăn truyền thống của Việt Nam ta và nổi tiếng với hương vị cuốn hút cũng như cách chế biến đơn giản. Vậy hiện nay du khách " +
                        "có thể thưởng thức món bánh tráng cuốn thịt heo tại những hàng quán nào tại Việt Nam? Hãy xem qua bài viết bên dưới của Traveloka nhé!\r\n\r\n" +
                        "Giới thiệu về bánh tráng cuốn thịt heo\r\n" +
                        "Bánh tráng cuốn thịt heo từ lâu đã được xem là món ngon tại xứ Quảng nói chung và tại Đà Nẵng nói riêng. Theo thời gian, sự hòa huyện của thịt " +
                        "ba chỉ, bánh tráng, rau thơm,... trong món bánh tráng cuốn thịt heo đã được mọi miền của tổ quốc biết đến.\r\n\r\n" +
                        "banh trang cuon thit heo\r\nMón bánh tráng cuốn thịt heo là đặc sản của xứ Quảng nói chung và Đà Nẵng nói riêng.@Shutterstock\r\n\r\n" +
                        "Nguyên liệu làm bánh tráng cuốn thịt heo\r\n" +
                        "Nguyên liệu tiên quyết của món ăn này chính là những thớ thịt ba chỉ béo ngậy. Ngoài ra, tùy vào từng vùng miền mà sẽ có thêm những nguyên" +
                        " liệu khác nhau. Nhưng một số nguyên liệu phổ biến nhất để tạo nên món ăn này có thể kể đến như:\r\n\r\n" +
                        "Bánh tráng phơi sương\r\nRau thơm\r\nXà lách\r\nXoài xanh\r\nTía tô\r\nChuối xanh\r\nKhế chua\r\nDứa\r\nMắm nêm\r\nỚt\r\n" +
                        "Cách làm bánh tráng cuốn thịt heo\r\nĐể tạo ra một món bánh tráng cuốn thịt heo “chuẩn gu” khá đơn giản. " +
                        "Bạn chỉ cần làm theo những bước sau:\r\n\r\nBước 1: Rửa sạch thịt heo và cho vào nồi luộc với một chút muối trên lửa nhỏ. Khi thịt mềm, " +
                        "vớt ra và cắt lát vừa ăn. Hãy đảm bảo rằng thịt được cắt ra có nạc và mỡ xen kẽ nhé\r\nBước 2: Đổ mắm nêm ra chén và cho thêm tỏi, ớt, dứa " +
                        "cùng 2 muỗng cà phê nước cốt chanh và 2 muỗng cà phê đường. Và sau đó trộn đều hỗn hợp này với nhau\r\nBước 3: Rửa sạch rau củ quả ăn kèm." +
                        " Bắt đầu bày biện các thành phần lên từng đĩa riêng\r\nBước 4: Bắt đầu cuốn thịt cũng như rau vào bánh tráng và thưởng thức món ăn\r\nbanh" +
                        " trang cuon thit heo\r\nĐây là món ăn giải nhiệt thanh đạm với nhiều rau tươi xanh.@Shutterstock\r\n\r\nCác địa điểm nổi tiếng để thưởng thức" +
                        " bánh tráng cuốn thịt heo\r\nBên cạnh việc tự tay tạo nên món bánh tráng cuốn thịt heo thơm ngon, thì bạn cũng có thể thưởng thức nó tại một " +
                        "số hàng quán nổi tiếng. Có thể kể đến như:\r\n\r\nCác quán bán bánh tráng cuốn thịt heo ở Hà Nội\r\nLiệu món ăn “quốc hồn quốc túy” của dân" +
                        " tộc Việt Nam sẽ có hương vị như thế nào tại mảnh đất nghìn năm văn hiến? Hãy đặt vé máy đi Hà Nội và trải nghiệm cũng như tìm câu trả lời tại" +
                        " những quán ăn này cùng Traveloka nhé:\r\n\r\n1. Cò Quán\r\n\r\nĐịa chỉ: 107 đường Hoàng Cầu, Đống Đa, Hà Nội\r\nGiá bán tham khảo:" +
                        " 20,000 - 350,000 VND\r\nNhờ vào vị trí tọa lạc ở mặt tiền đường, du khách sẽ không gặp quá nhiều khó khăn khi tìm đến Cò Quán để thưởng " +
                        "thức bánh tráng cuốn thịt heo. Một suất tại đây không chỉ có đa dạng các loại rau củ quả mà du khách còn có thể tùy chọn loại nhân khác nhau" +
                        ". Từ thịt luộc, thịt quay, nem nướng hay thậm chí là bắp giò luộc. Không chỉ đa dạng mà độ lớn của những miếng thịt tại quán cũng là điều giúp" +
                        " quán nhận về nhiều lời khen có cánh.\r\n\r\nbanh trang cuon thit heo\r\nCò Quán đem tới cho thực khách đa dạng các loại nhân khác nhau." +
                        "@Facebook CÒ QUÁN - 107 Hoàng Cầu - MÓN NGON ĐÀ NẴNG\r\n\r\n2. Đà Nẵng Quán\r\n\r\nĐịa chỉ: 79 đường Trung Hòa, quận Cầu Giấy, Hà Nội\r\nGiá bán tham khảo:" +
                        " 40,000 - 100,000 VND\r\nNhư tên gọi của mình, Đà Nẵng Quán sẽ đem đến cho du khách những món ăn trứ danh từ vùng đất của những cây cầu. Trong số các món" +
                        " ăn ấy thì không thể nào không nhắc đến bánh tráng cuốn thịt heo. Tại đây, du khách có thể tự do chọn giữa thịt luộc ngon ngọt hoặc thịt quay giòn rụm. " +
                        "Ngoài ra, mắm nêm tại quán cũng được làm theo hương vị riêng biệt. Từ đó đem đến trải nghiệm mới lạ cho món bánh tráng cuốn thịt heo.\r\n\r\n",
                        Author = "Nguyễn Thụy Mộc Nhiên",
                        PublishDate = new DateTime(2024,11,24),
                        Image = "ms-appx:///Assets/Blog/blog02.jpg"
                    },
                    new Blog
                    {
                        Id = 7,
                        Title = "Bánh tráng cuốn thịt heo - Món ăn đặc biệt từ Đà Nẵng",
                        Content = "Một chuyến du lịch Đà Nẵng trọn vẹn là một chuyến đi mà du khách đã thưởng thức món ăn dân dã mang tên bánh tráng cuốn thịt heo." +
                        " Đây là một món ăn truyền thống của Việt Nam ta và nổi tiếng với hương vị cuốn hút cũng như cách chế biến đơn giản. Vậy hiện nay du khách " +
                        "có thể thưởng thức món bánh tráng cuốn thịt heo tại những hàng quán nào tại Việt Nam? Hãy xem qua bài viết bên dưới của Traveloka nhé!\r\n\r\n" +
                        "Giới thiệu về bánh tráng cuốn thịt heo\r\n" +
                        "Bánh tráng cuốn thịt heo từ lâu đã được xem là món ngon tại xứ Quảng nói chung và tại Đà Nẵng nói riêng. Theo thời gian, sự hòa huyện của thịt " +
                        "ba chỉ, bánh tráng, rau thơm,... trong món bánh tráng cuốn thịt heo đã được mọi miền của tổ quốc biết đến.\r\n\r\n" +
                        "banh trang cuon thit heo\r\nMón bánh tráng cuốn thịt heo là đặc sản của xứ Quảng nói chung và Đà Nẵng nói riêng.@Shutterstock\r\n\r\n" +
                        "Nguyên liệu làm bánh tráng cuốn thịt heo\r\n" +
                        "Nguyên liệu tiên quyết của món ăn này chính là những thớ thịt ba chỉ béo ngậy. Ngoài ra, tùy vào từng vùng miền mà sẽ có thêm những nguyên" +
                        " liệu khác nhau. Nhưng một số nguyên liệu phổ biến nhất để tạo nên món ăn này có thể kể đến như:\r\n\r\n" +
                        "Bánh tráng phơi sương\r\nRau thơm\r\nXà lách\r\nXoài xanh\r\nTía tô\r\nChuối xanh\r\nKhế chua\r\nDứa\r\nMắm nêm\r\nỚt\r\n" +
                        "Cách làm bánh tráng cuốn thịt heo\r\nĐể tạo ra một món bánh tráng cuốn thịt heo “chuẩn gu” khá đơn giản. " +
                        "Bạn chỉ cần làm theo những bước sau:\r\n\r\nBước 1: Rửa sạch thịt heo và cho vào nồi luộc với một chút muối trên lửa nhỏ. Khi thịt mềm, " +
                        "vớt ra và cắt lát vừa ăn. Hãy đảm bảo rằng thịt được cắt ra có nạc và mỡ xen kẽ nhé\r\nBước 2: Đổ mắm nêm ra chén và cho thêm tỏi, ớt, dứa " +
                        "cùng 2 muỗng cà phê nước cốt chanh và 2 muỗng cà phê đường. Và sau đó trộn đều hỗn hợp này với nhau\r\nBước 3: Rửa sạch rau củ quả ăn kèm." +
                        " Bắt đầu bày biện các thành phần lên từng đĩa riêng\r\nBước 4: Bắt đầu cuốn thịt cũng như rau vào bánh tráng và thưởng thức món ăn\r\nbanh" +
                        " trang cuon thit heo\r\nĐây là món ăn giải nhiệt thanh đạm với nhiều rau tươi xanh.@Shutterstock\r\n\r\nCác địa điểm nổi tiếng để thưởng thức" +
                        " bánh tráng cuốn thịt heo\r\nBên cạnh việc tự tay tạo nên món bánh tráng cuốn thịt heo thơm ngon, thì bạn cũng có thể thưởng thức nó tại một " +
                        "số hàng quán nổi tiếng. Có thể kể đến như:\r\n\r\nCác quán bán bánh tráng cuốn thịt heo ở Hà Nội\r\nLiệu món ăn “quốc hồn quốc túy” của dân" +
                        " tộc Việt Nam sẽ có hương vị như thế nào tại mảnh đất nghìn năm văn hiến? Hãy đặt vé máy đi Hà Nội và trải nghiệm cũng như tìm câu trả lời tại" +
                        " những quán ăn này cùng Traveloka nhé:\r\n\r\n1. Cò Quán\r\n\r\nĐịa chỉ: 107 đường Hoàng Cầu, Đống Đa, Hà Nội\r\nGiá bán tham khảo:" +
                        " 20,000 - 350,000 VND\r\nNhờ vào vị trí tọa lạc ở mặt tiền đường, du khách sẽ không gặp quá nhiều khó khăn khi tìm đến Cò Quán để thưởng " +
                        "thức bánh tráng cuốn thịt heo. Một suất tại đây không chỉ có đa dạng các loại rau củ quả mà du khách còn có thể tùy chọn loại nhân khác nhau" +
                        ". Từ thịt luộc, thịt quay, nem nướng hay thậm chí là bắp giò luộc. Không chỉ đa dạng mà độ lớn của những miếng thịt tại quán cũng là điều giúp" +
                        " quán nhận về nhiều lời khen có cánh.\r\n\r\nbanh trang cuon thit heo\r\nCò Quán đem tới cho thực khách đa dạng các loại nhân khác nhau." +
                        "@Facebook CÒ QUÁN - 107 Hoàng Cầu - MÓN NGON ĐÀ NẴNG\r\n\r\n2. Đà Nẵng Quán\r\n\r\nĐịa chỉ: 79 đường Trung Hòa, quận Cầu Giấy, Hà Nội\r\nGiá bán tham khảo:" +
                        " 40,000 - 100,000 VND\r\nNhư tên gọi của mình, Đà Nẵng Quán sẽ đem đến cho du khách những món ăn trứ danh từ vùng đất của những cây cầu. Trong số các món" +
                        " ăn ấy thì không thể nào không nhắc đến bánh tráng cuốn thịt heo. Tại đây, du khách có thể tự do chọn giữa thịt luộc ngon ngọt hoặc thịt quay giòn rụm. " +
                        "Ngoài ra, mắm nêm tại quán cũng được làm theo hương vị riêng biệt. Từ đó đem đến trải nghiệm mới lạ cho món bánh tráng cuốn thịt heo.\r\n\r\n",
                        Author = "Nguyễn Thụy Mộc Nhiên",
                        PublishDate = new DateTime(2024,11,24),
                        Image = "ms-appx:///Assets/Blog/blog02.jpg"
                    },
                    new Blog
                    {
                        Id = 8,
                        Title = "Bánh tráng cuốn thịt heo - Món ăn đặc biệt từ Đà Nẵng",
                        Content = "Một chuyến du lịch Đà Nẵng trọn vẹn là một chuyến đi mà du khách đã thưởng thức món ăn dân dã mang tên bánh tráng cuốn thịt heo." +
                        " Đây là một món ăn truyền thống của Việt Nam ta và nổi tiếng với hương vị cuốn hút cũng như cách chế biến đơn giản. Vậy hiện nay du khách " +
                        "có thể thưởng thức món bánh tráng cuốn thịt heo tại những hàng quán nào tại Việt Nam? Hãy xem qua bài viết bên dưới của Traveloka nhé!\r\n\r\n" +
                        "Giới thiệu về bánh tráng cuốn thịt heo\r\n" +
                        "Bánh tráng cuốn thịt heo từ lâu đã được xem là món ngon tại xứ Quảng nói chung và tại Đà Nẵng nói riêng. Theo thời gian, sự hòa huyện của thịt " +
                        "ba chỉ, bánh tráng, rau thơm,... trong món bánh tráng cuốn thịt heo đã được mọi miền của tổ quốc biết đến.\r\n\r\n" +
                        "banh trang cuon thit heo\r\nMón bánh tráng cuốn thịt heo là đặc sản của xứ Quảng nói chung và Đà Nẵng nói riêng.@Shutterstock\r\n\r\n" +
                        "Nguyên liệu làm bánh tráng cuốn thịt heo\r\n" +
                        "Nguyên liệu tiên quyết của món ăn này chính là những thớ thịt ba chỉ béo ngậy. Ngoài ra, tùy vào từng vùng miền mà sẽ có thêm những nguyên" +
                        " liệu khác nhau. Nhưng một số nguyên liệu phổ biến nhất để tạo nên món ăn này có thể kể đến như:\r\n\r\n" +
                        "Bánh tráng phơi sương\r\nRau thơm\r\nXà lách\r\nXoài xanh\r\nTía tô\r\nChuối xanh\r\nKhế chua\r\nDứa\r\nMắm nêm\r\nỚt\r\n" +
                        "Cách làm bánh tráng cuốn thịt heo\r\nĐể tạo ra một món bánh tráng cuốn thịt heo “chuẩn gu” khá đơn giản. " +
                        "Bạn chỉ cần làm theo những bước sau:\r\n\r\nBước 1: Rửa sạch thịt heo và cho vào nồi luộc với một chút muối trên lửa nhỏ. Khi thịt mềm, " +
                        "vớt ra và cắt lát vừa ăn. Hãy đảm bảo rằng thịt được cắt ra có nạc và mỡ xen kẽ nhé\r\nBước 2: Đổ mắm nêm ra chén và cho thêm tỏi, ớt, dứa " +
                        "cùng 2 muỗng cà phê nước cốt chanh và 2 muỗng cà phê đường. Và sau đó trộn đều hỗn hợp này với nhau\r\nBước 3: Rửa sạch rau củ quả ăn kèm." +
                        " Bắt đầu bày biện các thành phần lên từng đĩa riêng\r\nBước 4: Bắt đầu cuốn thịt cũng như rau vào bánh tráng và thưởng thức món ăn\r\nbanh" +
                        " trang cuon thit heo\r\nĐây là món ăn giải nhiệt thanh đạm với nhiều rau tươi xanh.@Shutterstock\r\n\r\nCác địa điểm nổi tiếng để thưởng thức" +
                        " bánh tráng cuốn thịt heo\r\nBên cạnh việc tự tay tạo nên món bánh tráng cuốn thịt heo thơm ngon, thì bạn cũng có thể thưởng thức nó tại một " +
                        "số hàng quán nổi tiếng. Có thể kể đến như:\r\n\r\nCác quán bán bánh tráng cuốn thịt heo ở Hà Nội\r\nLiệu món ăn “quốc hồn quốc túy” của dân" +
                        " tộc Việt Nam sẽ có hương vị như thế nào tại mảnh đất nghìn năm văn hiến? Hãy đặt vé máy đi Hà Nội và trải nghiệm cũng như tìm câu trả lời tại" +
                        " những quán ăn này cùng Traveloka nhé:\r\n\r\n1. Cò Quán\r\n\r\nĐịa chỉ: 107 đường Hoàng Cầu, Đống Đa, Hà Nội\r\nGiá bán tham khảo:" +
                        " 20,000 - 350,000 VND\r\nNhờ vào vị trí tọa lạc ở mặt tiền đường, du khách sẽ không gặp quá nhiều khó khăn khi tìm đến Cò Quán để thưởng " +
                        "thức bánh tráng cuốn thịt heo. Một suất tại đây không chỉ có đa dạng các loại rau củ quả mà du khách còn có thể tùy chọn loại nhân khác nhau" +
                        ". Từ thịt luộc, thịt quay, nem nướng hay thậm chí là bắp giò luộc. Không chỉ đa dạng mà độ lớn của những miếng thịt tại quán cũng là điều giúp" +
                        " quán nhận về nhiều lời khen có cánh.\r\n\r\nbanh trang cuon thit heo\r\nCò Quán đem tới cho thực khách đa dạng các loại nhân khác nhau." +
                        "@Facebook CÒ QUÁN - 107 Hoàng Cầu - MÓN NGON ĐÀ NẴNG\r\n\r\n2. Đà Nẵng Quán\r\n\r\nĐịa chỉ: 79 đường Trung Hòa, quận Cầu Giấy, Hà Nội\r\nGiá bán tham khảo:" +
                        " 40,000 - 100,000 VND\r\nNhư tên gọi của mình, Đà Nẵng Quán sẽ đem đến cho du khách những món ăn trứ danh từ vùng đất của những cây cầu. Trong số các món" +
                        " ăn ấy thì không thể nào không nhắc đến bánh tráng cuốn thịt heo. Tại đây, du khách có thể tự do chọn giữa thịt luộc ngon ngọt hoặc thịt quay giòn rụm. " +
                        "Ngoài ra, mắm nêm tại quán cũng được làm theo hương vị riêng biệt. Từ đó đem đến trải nghiệm mới lạ cho món bánh tráng cuốn thịt heo.\r\n\r\n",
                        Author = "Nguyễn Thụy Mộc Nhiên",
                        PublishDate = new DateTime(2024,11,24),
                        Image = "ms-appx:///Assets/Blog/blog02.jpg"
                    },
                    new Blog
                    {
                        Id = 9,
                        Title = "Bánh tráng cuốn thịt heo - Món ăn đặc biệt từ Đà Nẵng",
                        Content = "Một chuyến du lịch Đà Nẵng trọn vẹn là một chuyến đi mà du khách đã thưởng thức món ăn dân dã mang tên bánh tráng cuốn thịt heo." +
                        " Đây là một món ăn truyền thống của Việt Nam ta và nổi tiếng với hương vị cuốn hút cũng như cách chế biến đơn giản. Vậy hiện nay du khách " +
                        "có thể thưởng thức món bánh tráng cuốn thịt heo tại những hàng quán nào tại Việt Nam? Hãy xem qua bài viết bên dưới của Traveloka nhé!\r\n\r\n" +
                        "Giới thiệu về bánh tráng cuốn thịt heo\r\n" +
                        "Bánh tráng cuốn thịt heo từ lâu đã được xem là món ngon tại xứ Quảng nói chung và tại Đà Nẵng nói riêng. Theo thời gian, sự hòa huyện của thịt " +
                        "ba chỉ, bánh tráng, rau thơm,... trong món bánh tráng cuốn thịt heo đã được mọi miền của tổ quốc biết đến.\r\n\r\n" +
                        "banh trang cuon thit heo\r\nMón bánh tráng cuốn thịt heo là đặc sản của xứ Quảng nói chung và Đà Nẵng nói riêng.@Shutterstock\r\n\r\n" +
                        "Nguyên liệu làm bánh tráng cuốn thịt heo\r\n" +
                        "Nguyên liệu tiên quyết của món ăn này chính là những thớ thịt ba chỉ béo ngậy. Ngoài ra, tùy vào từng vùng miền mà sẽ có thêm những nguyên" +
                        " liệu khác nhau. Nhưng một số nguyên liệu phổ biến nhất để tạo nên món ăn này có thể kể đến như:\r\n\r\n" +
                        "Bánh tráng phơi sương\r\nRau thơm\r\nXà lách\r\nXoài xanh\r\nTía tô\r\nChuối xanh\r\nKhế chua\r\nDứa\r\nMắm nêm\r\nỚt\r\n" +
                        "Cách làm bánh tráng cuốn thịt heo\r\nĐể tạo ra một món bánh tráng cuốn thịt heo “chuẩn gu” khá đơn giản. " +
                        "Bạn chỉ cần làm theo những bước sau:\r\n\r\nBước 1: Rửa sạch thịt heo và cho vào nồi luộc với một chút muối trên lửa nhỏ. Khi thịt mềm, " +
                        "vớt ra và cắt lát vừa ăn. Hãy đảm bảo rằng thịt được cắt ra có nạc và mỡ xen kẽ nhé\r\nBước 2: Đổ mắm nêm ra chén và cho thêm tỏi, ớt, dứa " +
                        "cùng 2 muỗng cà phê nước cốt chanh và 2 muỗng cà phê đường. Và sau đó trộn đều hỗn hợp này với nhau\r\nBước 3: Rửa sạch rau củ quả ăn kèm." +
                        " Bắt đầu bày biện các thành phần lên từng đĩa riêng\r\nBước 4: Bắt đầu cuốn thịt cũng như rau vào bánh tráng và thưởng thức món ăn\r\nbanh" +
                        " trang cuon thit heo\r\nĐây là món ăn giải nhiệt thanh đạm với nhiều rau tươi xanh.@Shutterstock\r\n\r\nCác địa điểm nổi tiếng để thưởng thức" +
                        " bánh tráng cuốn thịt heo\r\nBên cạnh việc tự tay tạo nên món bánh tráng cuốn thịt heo thơm ngon, thì bạn cũng có thể thưởng thức nó tại một " +
                        "số hàng quán nổi tiếng. Có thể kể đến như:\r\n\r\nCác quán bán bánh tráng cuốn thịt heo ở Hà Nội\r\nLiệu món ăn “quốc hồn quốc túy” của dân" +
                        " tộc Việt Nam sẽ có hương vị như thế nào tại mảnh đất nghìn năm văn hiến? Hãy đặt vé máy đi Hà Nội và trải nghiệm cũng như tìm câu trả lời tại" +
                        " những quán ăn này cùng Traveloka nhé:\r\n\r\n1. Cò Quán\r\n\r\nĐịa chỉ: 107 đường Hoàng Cầu, Đống Đa, Hà Nội\r\nGiá bán tham khảo:" +
                        " 20,000 - 350,000 VND\r\nNhờ vào vị trí tọa lạc ở mặt tiền đường, du khách sẽ không gặp quá nhiều khó khăn khi tìm đến Cò Quán để thưởng " +
                        "thức bánh tráng cuốn thịt heo. Một suất tại đây không chỉ có đa dạng các loại rau củ quả mà du khách còn có thể tùy chọn loại nhân khác nhau" +
                        ". Từ thịt luộc, thịt quay, nem nướng hay thậm chí là bắp giò luộc. Không chỉ đa dạng mà độ lớn của những miếng thịt tại quán cũng là điều giúp" +
                        " quán nhận về nhiều lời khen có cánh.\r\n\r\nbanh trang cuon thit heo\r\nCò Quán đem tới cho thực khách đa dạng các loại nhân khác nhau." +
                        "@Facebook CÒ QUÁN - 107 Hoàng Cầu - MÓN NGON ĐÀ NẴNG\r\n\r\n2. Đà Nẵng Quán\r\n\r\nĐịa chỉ: 79 đường Trung Hòa, quận Cầu Giấy, Hà Nội\r\nGiá bán tham khảo:" +
                        " 40,000 - 100,000 VND\r\nNhư tên gọi của mình, Đà Nẵng Quán sẽ đem đến cho du khách những món ăn trứ danh từ vùng đất của những cây cầu. Trong số các món" +
                        " ăn ấy thì không thể nào không nhắc đến bánh tráng cuốn thịt heo. Tại đây, du khách có thể tự do chọn giữa thịt luộc ngon ngọt hoặc thịt quay giòn rụm. " +
                        "Ngoài ra, mắm nêm tại quán cũng được làm theo hương vị riêng biệt. Từ đó đem đến trải nghiệm mới lạ cho món bánh tráng cuốn thịt heo.\r\n\r\n",
                        Author = "Nguyễn Thụy Mộc Nhiên",
                        PublishDate = new DateTime(2024,11,24),
                        Image = "ms-appx:///Assets/Blog/blog02.jpg"
                    },
                    new Blog
                    {
                        Id = 10,
                        Title = "Bánh tráng cuốn thịt heo - Món ăn đặc biệt từ Đà Nẵng",
                        Content = "Một chuyến du lịch Đà Nẵng trọn vẹn là một chuyến đi mà du khách đã thưởng thức món ăn dân dã mang tên bánh tráng cuốn thịt heo." +
                        " Đây là một món ăn truyền thống của Việt Nam ta và nổi tiếng với hương vị cuốn hút cũng như cách chế biến đơn giản. Vậy hiện nay du khách " +
                        "có thể thưởng thức món bánh tráng cuốn thịt heo tại những hàng quán nào tại Việt Nam? Hãy xem qua bài viết bên dưới của Traveloka nhé!\r\n\r\n" +
                        "Giới thiệu về bánh tráng cuốn thịt heo\r\n" +
                        "Bánh tráng cuốn thịt heo từ lâu đã được xem là món ngon tại xứ Quảng nói chung và tại Đà Nẵng nói riêng. Theo thời gian, sự hòa huyện của thịt " +
                        "ba chỉ, bánh tráng, rau thơm,... trong món bánh tráng cuốn thịt heo đã được mọi miền của tổ quốc biết đến.\r\n\r\n" +
                        "banh trang cuon thit heo\r\nMón bánh tráng cuốn thịt heo là đặc sản của xứ Quảng nói chung và Đà Nẵng nói riêng.@Shutterstock\r\n\r\n" +
                        "Nguyên liệu làm bánh tráng cuốn thịt heo\r\n" +
                        "Nguyên liệu tiên quyết của món ăn này chính là những thớ thịt ba chỉ béo ngậy. Ngoài ra, tùy vào từng vùng miền mà sẽ có thêm những nguyên" +
                        " liệu khác nhau. Nhưng một số nguyên liệu phổ biến nhất để tạo nên món ăn này có thể kể đến như:\r\n\r\n" +
                        "Bánh tráng phơi sương\r\nRau thơm\r\nXà lách\r\nXoài xanh\r\nTía tô\r\nChuối xanh\r\nKhế chua\r\nDứa\r\nMắm nêm\r\nỚt\r\n" +
                        "Cách làm bánh tráng cuốn thịt heo\r\nĐể tạo ra một món bánh tráng cuốn thịt heo “chuẩn gu” khá đơn giản. " +
                        "Bạn chỉ cần làm theo những bước sau:\r\n\r\nBước 1: Rửa sạch thịt heo và cho vào nồi luộc với một chút muối trên lửa nhỏ. Khi thịt mềm, " +
                        "vớt ra và cắt lát vừa ăn. Hãy đảm bảo rằng thịt được cắt ra có nạc và mỡ xen kẽ nhé\r\nBước 2: Đổ mắm nêm ra chén và cho thêm tỏi, ớt, dứa " +
                        "cùng 2 muỗng cà phê nước cốt chanh và 2 muỗng cà phê đường. Và sau đó trộn đều hỗn hợp này với nhau\r\nBước 3: Rửa sạch rau củ quả ăn kèm." +
                        " Bắt đầu bày biện các thành phần lên từng đĩa riêng\r\nBước 4: Bắt đầu cuốn thịt cũng như rau vào bánh tráng và thưởng thức món ăn\r\nbanh" +
                        " trang cuon thit heo\r\nĐây là món ăn giải nhiệt thanh đạm với nhiều rau tươi xanh.@Shutterstock\r\n\r\nCác địa điểm nổi tiếng để thưởng thức" +
                        " bánh tráng cuốn thịt heo\r\nBên cạnh việc tự tay tạo nên món bánh tráng cuốn thịt heo thơm ngon, thì bạn cũng có thể thưởng thức nó tại một " +
                        "số hàng quán nổi tiếng. Có thể kể đến như:\r\n\r\nCác quán bán bánh tráng cuốn thịt heo ở Hà Nội\r\nLiệu món ăn “quốc hồn quốc túy” của dân" +
                        " tộc Việt Nam sẽ có hương vị như thế nào tại mảnh đất nghìn năm văn hiến? Hãy đặt vé máy đi Hà Nội và trải nghiệm cũng như tìm câu trả lời tại" +
                        " những quán ăn này cùng Traveloka nhé:\r\n\r\n1. Cò Quán\r\n\r\nĐịa chỉ: 107 đường Hoàng Cầu, Đống Đa, Hà Nội\r\nGiá bán tham khảo:" +
                        " 20,000 - 350,000 VND\r\nNhờ vào vị trí tọa lạc ở mặt tiền đường, du khách sẽ không gặp quá nhiều khó khăn khi tìm đến Cò Quán để thưởng " +
                        "thức bánh tráng cuốn thịt heo. Một suất tại đây không chỉ có đa dạng các loại rau củ quả mà du khách còn có thể tùy chọn loại nhân khác nhau" +
                        ". Từ thịt luộc, thịt quay, nem nướng hay thậm chí là bắp giò luộc. Không chỉ đa dạng mà độ lớn của những miếng thịt tại quán cũng là điều giúp" +
                        " quán nhận về nhiều lời khen có cánh.\r\n\r\nbanh trang cuon thit heo\r\nCò Quán đem tới cho thực khách đa dạng các loại nhân khác nhau." +
                        "@Facebook CÒ QUÁN - 107 Hoàng Cầu - MÓN NGON ĐÀ NẴNG\r\n\r\n2. Đà Nẵng Quán\r\n\r\nĐịa chỉ: 79 đường Trung Hòa, quận Cầu Giấy, Hà Nội\r\nGiá bán tham khảo:" +
                        " 40,000 - 100,000 VND\r\nNhư tên gọi của mình, Đà Nẵng Quán sẽ đem đến cho du khách những món ăn trứ danh từ vùng đất của những cây cầu. Trong số các món" +
                        " ăn ấy thì không thể nào không nhắc đến bánh tráng cuốn thịt heo. Tại đây, du khách có thể tự do chọn giữa thịt luộc ngon ngọt hoặc thịt quay giòn rụm. " +
                        "Ngoài ra, mắm nêm tại quán cũng được làm theo hương vị riêng biệt. Từ đó đem đến trải nghiệm mới lạ cho món bánh tráng cuốn thịt heo.\r\n\r\n",
                        Author = "Nguyễn Thụy Mộc Nhiên",
                        PublishDate = new DateTime(2024,11,24),
                        Image = "ms-appx:///Assets/Blog/blog02.jpg"
                    }
                };
            return result;
        }
        public List<Blog> GetLastestBlog()
        {
            List<Blog> result = new List<Blog>();
            for (int i = 0; i < 3; i++)
            {
                result.Add(GetAllBlog()[i]);
            }
            return result;
        }

        public Blog GetBlogById(int id)
        {
            return GetAllBlog().FirstOrDefault(x => x.Id == id);
        }
    }
}
