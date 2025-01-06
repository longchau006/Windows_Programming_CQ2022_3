using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows_Programming.Database;
using Windows_Programming.Model;

namespace Windows_Programming.ViewModel
{
    public class TourViewModel
    {
        public List<Tour> AllTour { get; set; }
        public List<Tour> AllTour_Copy { get; set; }

        /*public async Task AddTour()
        {
            IDao dao = FirebaseServicesDAO.Instance;
            await dao.AddTour(new Tour
            {
                Name = "Khám phá 2 đảo, Sun World Hòn Thơm, Bữa trưa buffet tại Phú Quốc - Tour 1 ngày của Rooty Trip",
                Places = "Phú Quốc, Phu Quoc, Kien Giang, Vietnam",
                Description = "Bạn sẽ trải nghiệm\r\nKhám phá phía Nam đảo Phú Quốc trong 1 ngày\r\nKhám phá những hòn đảo nhỏ nhưng hùng vĩ đã tạo nên tên tuổi của Phú Quốc ngày nay: một chốn thiên đường của biển khơi\r\nChiêm ngưỡng vẻ đẹp Sun World Hòn Thơm\r\nTham gia các trò chơi nước cực sảng khoái tại Aquatopia—một trong những công viên nước hiện đại nhất Đông Nam Á\r\nTour sẽ đưa bạn đi tham quan 2 trong số các đảo nhỏ đẹp nhất phía nam đảo Phú Quốc, nơi bạn sẽ không khỏi trầm trồ trước những bãi biển cát trắng hoang sơ và hệ sinh vật biển phong phú.\r\n\r\nHãy phiêu lưu, lặn sâu để ngắm các rạn san hô, nằm dài trên bãi cát ngập nắng, chụp thật nhiều ảnh. Dù có làm bất cứ điều gì, thì bạn cũng không thể bỏ qua được tour này. Vì đây là Phú Quốc, và thiên đường chỉ có thể đẹp như thế này mà thôi. ",
                Price = 1300000,
                Rating = 10,
                Schedule = "07:30-17:00 Đón khách\r\nTham quan trung tâm trưng bày ngọc trai Longbeach Center \r\nDi chuyển bằng ca nô đến Hòn Mây Rút hoặc Hòn Móng Tay nơi khách tự do bơi lội, chụp ảnh flycam, chèo SUP \r\nTham quan công viên san hô và thử lặn ống thở ngắm san hô (đã bao gồm trong vé)\r\nDịch vụ đi bộ dưới biển có sẵn với chi phí tự túc \r\nĐến Sun World Hòn Thơm, vui chơi tại công viên nước Aquatopia \r\nDùng bữa trưa buffet tại nhà hàng Coconut Garden\r\nVui chơi tại Hòn Thơm đến khi kết thúc chương trình và trở về Phú Quốc \r\nKết thúc tour. ",
                Image = "D:/CS/LTWindows/Windows_Programming/Windows_Programming/Windows_Programming/Assets/Tour/tour01.jpg",
                Link = "https://www.traveloka.com/vi-vn/activities/vietnam/product/two-islands-excursion-sun-world-hon-thom-lunch-buffet-in-phu-quoc-by-rooty-trip-day-tour-3658898400568"
            });
            await dao.AddTour(new Tour
            {
                Name = "Khám phá Rặng San Hô - Làng Chài - Bãi Tranh tại Nha Trang - Tour 1 ngày",
                Places = "Tp. Nha Trang, Khánh Hòa, Việt Nam",
                Description = "Bạn sẽ trải nghiệm\r\nTrải nghiệm một chuyến đi khám phá cuộc sống ở đảo đích thực\r\nTham quan Rặng San Hô–điểm đến lý tưởng với các hoạt dộng vui chơi như: Chèo thuyền kayak, chèo SUP, nhà phao trên biển, bơi ngắm san hô, lặn biển,...\r\nBơi giữa các rặng san hô và các loài sinh vật biển\r\nNằm dài trên bãi biển, tận hưởng ánh nắng ấm áp\r\nHãy dành ra một ngày không cần lo nghĩ hay ngập sâu trong lịch trình bận rộn của bạn để tham gia chuyến đi đảo tại Nha Trang, nơi bạn sẽ được nuông chiều bởi cái mát lạnh của nước biển trong vắt, âm thanh êm dịu của sóng biển và ngủ một giấc trưa thư thái trong làn gió ấm áp.\r\n\r\nVới một loạt các hoạt động thân thiện với gia đình và bạn bè, bạn sẽ nhận ra rằng thời gian bên cạnh những người thân yêu làm những điều thú vị là quãng thời gian xứng đáng nhất. ",
                Price = 553500,
                Rating = 9,
                Schedule = "08:00-15:30 Xe cùng hướng dẫn viên đón khách tại khách sạn (trong nội thành Nha Trang), xe đưa khách đến bến cảng\r\nĐến bến cảng, du khách di chuyển bằng ca nô/ thuyền, bắt đầu chuyến tham quan tour ba đảo Nha Trang. Trên đường đi du khách có thể ngắm nhìn cảnh biển vịnh Nha Trang xinh đẹp.\r\nĐến Rặng San Hô. khách tự do khám phá vịnh. \r\nTrải nghiệm nhiều dịch vụ trò chơi trên biển như: lặn biển, đi bộ dưới đáy biển, dù bay, mô tô nước,...(chi phí tự túc)\r\nKhách di chuyển đến Làng Chài, khách sẽ được tham quan hệ thống lồng bè nuôi trồng thủy sản có quy mô lớn nhất Vịnh Nha Trang. Sau đó thưởng thức bữa trưa 10 món. \r\nDi chuyển đến Bãi Tranh.\r\nKhách tự do bơi lội, tắm nắng hoặc tham gia nhiều hoạt động thể thao bãi biển tại đây (chi phí tự túc).\r\nCa nô/ thuyền đưa du khách về lại cảng. Xe đưa du khách về lại khách sạn. Kết thúc tour. ",
                Image = "D:/CS/LTWindows/Windows_Programming/Windows_Programming/Windows_Programming/Assets/Tour/tour02.jpg",
                Link = "https://www.traveloka.com/vi-vn/activities/vietnam/product/coral-reef-fishing-village-tranh-beach-discovery-in-nha-trang-day-tour-4486341440428"
            });
            await dao.AddTour(new Tour
            {
                Name = "Tour khám phá Bà Nà Hills (Cầu Vàng Đà Nẵng) - 1 ngày",
                Places = "Hoà Ninh, Hòa Vang, Đà Nẵng, Vietnam",
                Description = "Bạn sẽ trải nghiệm\r\nTận hưởng khung cảnh tuyệt vời của đỉnh Bà Nà đứng trên cầu Vàng, một trong những chiếc cầu đẹp nhất thế giới\r\nNgắm nhìn vẻ đẹp hùng vĩ của núi Chúa từ buồng cáp treo\r\nGhé thăm làng Pháp với những khu vườn theo kiến trúc Pháp tinh tế\r\nVui chơi thoả thích tại công viên Fantasy Park\r\nBà Nà Hills là khu phức hợp giải trí và resort lớn nhất tại Việt Nam. Cùng nhau đi tour và xả láng cả ngày tại Bà Nà Hills ngay nào! Tận hưởng không khí mát lạnh cùng phong cảnh tuyệt vời, ăn hết mình với đủ loại ẩm thực và chơi hết sức với những lễ hội và các hoạt động giải trí đa dạng diễn ra hằng ngày, tất cả đều ngay tại đây!",
                Schedule = "07:30-17:00 Đón khách \r\nĐến Bà Nà Hills, di chuyển bằng cáp treo \r\nTham quan Bà Nà Hills và check in cầu vàng, vườn Le Jardin D'Amour, chùa Linh Ứng, hầm rượu Debay \r\nDi chuyển tiếp bằng cáp treo lên đến đỉnh núi \r\nDùng bữa trưa trên đỉnh Núi Chúa \r\nTiếp tục khám phá Bà Nà \r\nRa về bằng cáp treo\r\nTrở về trung tâm Đà Nẵng\r\nKết thúc tour. ",
                Price = 722500,
                Rating = 9,
                Image = "D:/CS/LTWindows/Windows_Programming/Windows_Programming/Windows_Programming/Assets/Tour/tour03.jpg",
                Link = "https://www.traveloka.com/vi-vn/activities/vietnam/product/ba-na-hills-vietnam-golden-bridge-day-tour-2001357730516"

            });
            await dao.AddTour(new Tour
            {
                Name = "Vé VinWonders Nam Hội An",
                Places = "Bình Minh, Thăng Bình District, Quang Nam Province 550000, Vietnam",
                Description = "ạn sẽ trải nghiệm\r\nDu ngoạn trên sông, thảnh thơi trải nghiệm thế giới động vật bán hoang dã\r\nTham gia vào các trải nghiệm và sự kiện mỗi mùa, tìm hiểu về sự đa dạng văn hóa và nghề thủ công của Việt Nam\r\nNgược dòng lịch sử trong không gian nghệ thuật truyền thống với các show diễn thực cảnh và các chương trình thú vị khác\r\nMiễn phí dịch vụ trung chuyển cho khách tại Đà Nẵng và Hội An\r\nBấm vào để xem các hoạt động và điểm tham quan tại: River Safari, Đảo Văn hóa Dân gian, tổng thể VinWonders Nam Hội An. \r\nĐọc thêm về các hoạt động và điểm than quan tại công viên. \r\n\r\nNổi tiếng với phố cổ quyến rũ và bí ẩn, giờ đây Hội An còn mang đến nhiều hoạt động thú vị hơn nữa để bạn trải nghiệm\r\n\r\nNằm ở vùng ven biển đẹp như tranh vẽ, VinWonders Nam Hội An có tầm nhìn tuyệt đẹp ra cảnh quan xung quanh. Bạn và những người thân yêu sẽ được tận hưởng khung cảnh xanh tươi, những bãi biển cát mịn, những vùng nước thanh bình và các hoạt động dưới nước với nhiều hồ bơi, đường trượt, dòng sông lười và khu vực té nước. \r\n\r\n\r\n\r\n\r\n\r\n\r\nĐối với các gia đình có trẻ em, Junior Zookeeper là hoạt động nhập vai cho phép các bé trở thành “bảo mẫu chăm sóc động vật nhí” trong một ngày và tham gia vào hoạt động bảo tồn thiên nhiên. Hoạt động này sẽ cho phép trẻ em của bạn học thêm về: \r\n\r\nTHIÊN NHIÊN: Đưa các bé đại náo khu vườn xanh tươi Happy Zoo, khám phá nhà chung của muôn loài động vật đáng yêu như dê lùn Pugmy, hươu sao nhút nhát và đà điểu châu Phi cao lớn. \r\nTÍNH TỰ LẬP: Nuôi dưỡng bản lĩnh và tinh thần trách nhiệm trong con từ các nhiệm vụ nhỏ như làm đẹp nơi ở hay cho các bạn động vật “măm măm” dưới sự hướng dẫn của cô chú Chuyên viên thực thụ. \r\nSÁNG TẠO: Kích thích trí sáng tạo và tinh thần đồng đội của các Zoo Keeper nhí qua những trò chơi thử thách như tìm trứng và ghép hình các bạn động vật xinh yêu. \r\n“Flex” nhẹ tấm bằng Junior Zookeeper có 1-0-2 chứng nhận hành trình trải nghiệm và ghi dấu mùa hè tuyệt vời của con! Đừng bỏ lỡ thời điểm vàng cho con vui hết mình và thoả thích khám phá những trải nghiệm chưa từng có tại Happy Zoo - VinWonders Nam Hội An bố mẹ nhé!\r\n\r\n\r\nTận hưởng nhiều hoạt động dưới nước xua tan nắng nóng miền Trung\r\n\r\n\r\nĐừng quên trải nghiệm River Safari, nơi mang đến chuyến đi trên sông độc đáo để khám phá đời sống hoang dã đa dạng\r\n\r\n\r\nNgoài việc là một công viên giải trí, tại VinWonders Nam Hội An, bạn sẽ được tham gia các chuyến tham quan nhỏ và chứng kiến các chương trình giải trí, biểu diễn văn hóa suốt cả ngày. Những chương trình có thể bao gồm các show diễn hoạt cảnh, nhào lộn và trình diễn văn hóa.\r\n\r\n\r\nSuỵt! Chúa tể sơn lâm đang nghỉ ngơi. Đừng làm ồn nha!\r\n\r\n\r\nLắng nghe câu chuyện giải cứu, bảo tồn, nuôi dưỡng và chăm sóc động vật của Safari Nam Hội An — Công viên bảo tồn động vật hoang dã trên sông đầu tiên và duy nhất tại Việt Nam.\r\n\r\nVới đường đi dễ dàng từ nhiều địa điểm khác nhau ở miền Trung Việt Nam, bao gồm thành phố Đà Nẵng và thị trấn Hội An, không có lý do gì để không đưa nơi đây vào danh sách những nơi phải đến, dù-chỉ-1-lần, khi bạn đang tận hưởng một kỳ nghỉ vô cùng xứng đáng tại miền Trung thanh bình, giản dị.",
                Price = 250000,
                Rating = 9,
                Image = "D:/CS/LTWindows/Windows_Programming/Windows_Programming/Windows_Programming/Assets/Tour/tour04.jpg",
                Schedule = "Tự do",
                Link = "https://www.traveloka.com/vi-vn/activities/vietnam/product/vinwonders-nam-hoi-an-tickets-2000684220694"
            });
            await dao.AddTour(new Tour
            {
                Name = "Chinh phục Langbiang, thác Datanla và Crazy House - Tour 1 ngày",
                Description = "Bạn sẽ trải nghiệm\r\nTham quan một số điểm đáng đến nhất tại thành phố của sương mù và tình yêu Đà Lạt\r\nChinh phục đỉnh núi Langbiang, đỉnh núi cao nhất tại Đà Lạt, với chuyến xe jeep lên đỉnh thú vị\r\nKhám phá thác Datanla, nơi bạn có thể trải nghiệm cảm giác hồi hộp khi chơi máng trượt dài đến 2400 m\r\nNgắm toàn cảnh núi Langbiang, hồ Xuân Hương, và hồ Tuyền Lâm đứng trên đòi Robin\r\nBạn đã sẵn sàng cho chuyến đi tiếp theo tới Đà Lạt chưa?\r\n\r\nĐà Lạt là một thành phố xinh đẹp, và chiêm ngưỡng những ngõ ngách thú vị của nơi đây qua các hoạt động trong tour này là một trải nghiệm hoàn toàn đáng có. Bạn sẽ được đến thăm thác Datanla, nơi bạn có thể thử sức băng qua những cánh rừng thông hùng vĩ cùng với những thác nước đẹp và khung cảnh đẹp đến mê hồn trên ván trượt, hoặc chinh phục Langbiang—ngọn núi cao nhất thành phố, hoặc lên đồi Robin để nhìn ngắm những thung lũng và ngọn đồi hiện lên sống động ngay trước mắt bạn, hoặc chỉ đơn giản là relax và hít thở không khí trong lành và ngắm nhìn cảnh quan thiên nhiên mà viên ngọc quý Đà Lạt của núi rừng Tây Nguyên đã mang lại cho chúng ta. ",
                Price = 500000,
                Rating = 9,
                Schedule = "08:00-16:00 Chinh phục núi LangBiang - nóc nhà Tây Nguyên (cao 1929m) (dịch vụ xe jeep lên đỉnh núi tự túc) \r\nĐồi Robin (dịch vụ cáp treo tự túc) \r\nThiền viện Trúc Lâm – Hồ Tuyền Lâm\r\nThác Datanla (trải nghiệm máng trượt dài nhất Đông Nam Á với chiều dài 2400m với chi phí tự túc) \r\nCrazy House – Biệt thự Hằng Nga (Ngôi nhà Điên Đà Lạt)\r\nVườn dâu tây công nghệ cao ",
                Image = "D:/CS/LTWindows/Windows_Programming/Windows_Programming/Windows_Programming/Assets/Tour/tour05.jpg",
                Places = "QL20 Đèo Prenn, Phường 3, Thành phố Đà Lạt, Lâm Đồng 66000, Vietnam",
                Link = "https://www.traveloka.com/vi-vn/activities/vietnam/product/langbiang-datanla-waterfalls-and-crazy-house-adventure-day-tour-4091396748660"
            });
            await dao.AddTour(new Tour
            {
                Name = "Tour Hoa Lư, Tràng An, và Hang Múa - 1 ngày",
                Places = "Tràng An, Tân Thành, Tp. Ninh Bình, Ninh Bình, Vietnam",
                Description = "Bạn sẽ trải nghiệm\r\nTham quan Quần thể danh thắng Tràng An, di sản văn hóa và thiên nhiên thế giới do UNESCO công nhận\r\nTìm hiểu các di tích lịch sử của cố đô Hoa Lư\r\nThăm đền thờ vua Đinh Tiên Hoàng và vua Lê Đại Hành\r\nThưởng ngoạn quang cảnh hùng vĩ và hoang sơ từ đỉnh Hang Múa\r\nĐược người dân Việt Nam thân thương gọi là \"Hạ Long trên cạn,\" Ninh Bình có thể được gọi là người em gái hiền lành nhưng không kém phần quyến rũ của người chị là vịnh Hạ Long. Nổi tiếng với rừng quốc gia, hang động, dòng sông yên bình và các di tích lịch sử, đã đến lúc Ninh Bình có được sự công nhận tương xứng với vẻ đẹp của nơi này.\r\n\r\n\r\nThuyền sẽ đi dọc sông Tràng An, đưa bạn từ hang động bí ẩn này đến hang động đáng ngạc nhiên khác \r\n\r\nÝ nghĩa lịch sử: Chuyến thăm đầu tiên của bạn là di tích Hoa Lư. Từng là thủ đô của Việt Nam, Hoa Lư mang theo một quá khứ huy hoàng và một cảm giác hoài niệm đầy chất thơ. Đạp xe qua Hoa Lư và tìm hiểu về các di tích lịch sử, ghé thăm đền thờ vua Đinh Tiên Hoàng và Lê Đại Hạnh, và tận hưởng sự yên tĩnh của vùng đồng quê.\r\n\r\nĐạp xe qua vùng nông thôn và trải nghiệm lối sống yên bình đích thực của miền Bắc Việt Nam\r\n\r\nThắng cảnh thiên nhiên: Khám phá Quần thể danh thắng Tràng An, di sản văn hóa và thiên nhiên thế giới do UNESCO công nhận. Ngồi trên thuyền và hãy nhìn lên và xung quanh bạn, bởi vì bạn sẽ được bao quanh với những cánh đồng lúa tuyệt đẹp, những ngọn núi đá vôi hùng vĩ và một bầu trời xanh ngắt. Kết thúc một ngày đáng nhớ với một chuyến leo bậc thang lên đỉnh Hang Múa, nơi bạn có thể có tầm nhìn 360 độ của một Tràng An hoang sơ và đẹp đến nao lòng.\r\n\r\nThử thách bản thân leo thang dốc lên đỉnh Hang Múa. Không dễ dàng chút nào, nhưng khung cảnh ngoạn mục từ trên cao hoàn toàn xưng đáng cho những giọt mồ hôi của bạn\r\n\r\nHãy đến, và khám phá vẻ đẹp thực sự của miền Bắc Việt Nam.",
                Schedule = "•\r\n07:00-10:30 Đón khách và di chuyển về Ninh Bình. Dừng chân khoảng 20 phút nghỉ ngơi trên đường đi.\r\n•\r\n10:30-11:45 Khách thăm quan cố đô Hoa Lư, tìm hiểu về lịch sử các triều đại phong kiến trị vì Việt Nam trong suốt 41 năm, từ 968 – 1010. \r\n•\r\n11:45-13:00 Dùng bữa trưa với đa dạng các món ăn địa phương. \r\n•\r\n13:00-15:30 Khách tiếp tục đi thuyền thăm quan Tràng An — di sản thiên nhiên thế giới được UNESCO công nhận năm 2014, nổi tiếng với hệ thống hang động tuyệt đẹp, với hơn 2 giờ đồng hồ chiêm ngưỡng vẻ đẹp nơi đây. \r\n•\r\n15:30-16:30 Trở lại xe và tiếp tục hành trình đi thăm Hang Múa, đỉnh cao nhất của Tam Cốc. Vượt qua gần 500 bậc đá khác nhau, khách sẽ tiến lên tới đỉnh cao nhất, ngắm nhìn khung cảnh tuyệt đẹp của Tam Cốc từ trên cao.\r\n•\r\n16:30-19:00 Khách lên xe trở về Hà Nội. Kết thúc tour.",
                Price = 840222,
                Rating = 10,
                Image = "D:/CS/LTWindows/Windows_Programming/Windows_Programming/Windows_Programming/Assets/Tour/tour06.jpg",
                Link = "https://www.traveloka.com/vi-vn/activities/vietnam/product/hoa-lu-trang-an-and-mua-cave-day-tour-2000808458945"
            });
            await dao.AddTour(new Tour
            {
                Name = "Tour khám phá Bà Nà Hills (Cầu Vàng Đà Nẵng) - 1 ngày",
                Places = "Hoà Ninh, Hòa Vang, Đà Nẵng, Vietnam",
                Description = "Bạn sẽ trải nghiệm\r\nTận hưởng khung cảnh tuyệt vời của đỉnh Bà Nà đứng trên cầu Vàng, một trong những chiếc cầu đẹp nhất thế giới\r\nNgắm nhìn vẻ đẹp hùng vĩ của núi Chúa từ buồng cáp treo\r\nGhé thăm làng Pháp với những khu vườn theo kiến trúc Pháp tinh tế\r\nVui chơi thoả thích tại công viên Fantasy Park\r\nBà Nà Hills là khu phức hợp giải trí và resort lớn nhất tại Việt Nam. Cùng nhau đi tour và xả láng cả ngày tại Bà Nà Hills ngay nào! Tận hưởng không khí mát lạnh cùng phong cảnh tuyệt vời, ăn hết mình với đủ loại ẩm thực và chơi hết sức với những lễ hội và các hoạt động giải trí đa dạng diễn ra hằng ngày, tất cả đều ngay tại đây!",
                Schedule = "07:30-17:00 Đón khách \r\nĐến Bà Nà Hills, di chuyển bằng cáp treo \r\nTham quan Bà Nà Hills và check in cầu vàng, vườn Le Jardin D'Amour, chùa Linh Ứng, hầm rượu Debay \r\nDi chuyển tiếp bằng cáp treo lên đến đỉnh núi \r\nDùng bữa trưa trên đỉnh Núi Chúa \r\nTiếp tục khám phá Bà Nà \r\nRa về bằng cáp treo\r\nTrở về trung tâm Đà Nẵng\r\nKết thúc tour. ",
                Price = 722500,
                Rating = 9,
                Image = "D:/CS/LTWindows/Windows_Programming/Windows_Programming/Windows_Programming/Assets/Tour/tour03.jpg",
                Link = "https://www.traveloka.com/vi-vn/activities/vietnam/product/ba-na-hills-vietnam-golden-bridge-day-tour-2001357730516"

            });
            await dao.AddTour(new Tour
            {
                Name = "Tour khám phá Bà Nà Hills (Cầu Vàng Đà Nẵng) - 1 ngày",
                Places = "Hoà Ninh, Hòa Vang, Đà Nẵng, Vietnam",
                Description = "Bạn sẽ trải nghiệm\r\nTận hưởng khung cảnh tuyệt vời của đỉnh Bà Nà đứng trên cầu Vàng, một trong những chiếc cầu đẹp nhất thế giới\r\nNgắm nhìn vẻ đẹp hùng vĩ của núi Chúa từ buồng cáp treo\r\nGhé thăm làng Pháp với những khu vườn theo kiến trúc Pháp tinh tế\r\nVui chơi thoả thích tại công viên Fantasy Park\r\nBà Nà Hills là khu phức hợp giải trí và resort lớn nhất tại Việt Nam. Cùng nhau đi tour và xả láng cả ngày tại Bà Nà Hills ngay nào! Tận hưởng không khí mát lạnh cùng phong cảnh tuyệt vời, ăn hết mình với đủ loại ẩm thực và chơi hết sức với những lễ hội và các hoạt động giải trí đa dạng diễn ra hằng ngày, tất cả đều ngay tại đây!",
                Schedule = "07:30-17:00 Đón khách \r\nĐến Bà Nà Hills, di chuyển bằng cáp treo \r\nTham quan Bà Nà Hills và check in cầu vàng, vườn Le Jardin D'Amour, chùa Linh Ứng, hầm rượu Debay \r\nDi chuyển tiếp bằng cáp treo lên đến đỉnh núi \r\nDùng bữa trưa trên đỉnh Núi Chúa \r\nTiếp tục khám phá Bà Nà \r\nRa về bằng cáp treo\r\nTrở về trung tâm Đà Nẵng\r\nKết thúc tour. ",
                Price = 722500,
                Rating = 9,
                Image = "D:/CS/LTWindows/Windows_Programming/Windows_Programming/Windows_Programming/Assets/Tour/tour03.jpg",
                Link = "https://www.traveloka.com/vi-vn/activities/vietnam/product/ba-na-hills-vietnam-golden-bridge-day-tour-2001357730516"

            });
        }*/

        public TourViewModel()
        {
            AllTour = new List<Tour>();
            AllTour_Copy = new List<Tour>();
        }

        public async Task GetAllTour()
        {
            IDao dao = FirebaseServicesDAO.Instance;
            AllTour = await dao.GetAllTour(); ;
            foreach (var tour in AllTour)
            {
                AllTour_Copy.Add(tour);
            }
        }

        public Tour Tour { get; set; }

        public async Task GetTourById(string id)
        {
            IDao dao = FirebaseServicesDAO.Instance;
            Tour = await dao.GetTourById(id);
        }

        public void searchTour(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                AllTour = AllTour_Copy;
            }
            else
            {
                AllTour = AllTour_Copy.Where(tour => tour.Name.Contains(keyword)).ToList();
            }
        }
    }
}
