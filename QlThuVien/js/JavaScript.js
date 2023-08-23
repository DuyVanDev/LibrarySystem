var start = {
    init: function () {
        start.requestEvent();
    },
    requestEvent: function () {
        $(".btn-borrow").on("click", function (e) {
            e.preventDefault();
            var productId = $(".productId").data("bookid");
            var userid = ($(this).data("userid"))
            console.log(userid)
            if (userid == "") {
                window.location.href = "/User/DangNhap"
            }
            
        })
    }
}
start.init();