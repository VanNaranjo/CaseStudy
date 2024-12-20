$(() => { // main jQuery routine - executes every on page load, $ is short for jquery
    $("#dialog").hide();
    $("#dialogbutton").on("click", (e) => {
        $("#dialog").show();
        $("#status").text("");
        $("#dialogbutton").hide();
    });
    $("#nobutton").on("click", (e) => {
        $("#dialog").hide();
        $("#status").text("second thoughts eh?");
        $("#dialogbutton").show();
    });
    $("#yesbutton").on("click", () => {
        $("#dialog").hide();
        $("#status").text("way to go for it!");
        $("#dialogbutton").show();
    });
}); // jQuery ready method