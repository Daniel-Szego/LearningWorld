

// Mouse leaving the star zone
function OnMouseLeave() {
    SetNthStar(getNthStarNum()); 
}


// Mouse enter
function OnMouseEnter() {
    var num = $(this).attr('num');
    for (var i = 1; i < 6; i++) {
        var numString = "#star" + i;
        if (i <= num) {
            $(numString).attr("src", "/_layouts/Lernwelt.SharePoint.Web/RatingsNew.png");
        }
        else {
            $(numString).attr("src", "/_layouts/Lernwelt.SharePoint.Web/RatingsEmpty.png");
        }
    }
}


// Click
function OnClick() {
    var num = $(this).attr('num');
    SetNthStar(num);
    setNthStarNum(num);
}

// setting active stars
function SetNthStar(nth) {
    if (typeof nth == 'undefined')
        nth = 0;
    if (nth > 6 || nth < 1)
        nth = 0;
    for (var i = 1; i < 6; i++) {
        var numString = "#star" + i;
        if (i <= nth) {
            $(numString).attr("src", "/_layouts/Lernwelt.SharePoint.Web/RatingsNew.png");
        }
        else {
            $(numString).attr("src", "/_layouts/Lernwelt.SharePoint.Web/RatingsEmpty.png");
        }
    }
}


// getting the Star number from the hidden storage
function getNthStarNum() {
    var NthStarNum = $("[id*=hdnStarNum]").attr("Value");
//    alert("getting hidden: " + NthStarNum);
    return (NthStarNum);
 }


// setting the Star number in the hidden storage
function setNthStarNum(newStarNum) {
    $("[id*=hdnStarNum]").attr("Value", newStarNum);
//    alert("setting hidden new : " + newStarNum);
 }



// eventhandlers init
function InitializeRating() {

    // event receivers
    $("#star1").click(OnClick);
    $("#star2").click(OnClick);
    $("#star3").click(OnClick);
    $("#star4").click(OnClick);
    $("#star5").click(OnClick);

    $("#star1").mouseenter(OnMouseEnter);
    $("#star2").mouseenter(OnMouseEnter);
    $("#star3").mouseenter(OnMouseEnter);
    $("#star4").mouseenter(OnMouseEnter);
    $("#star5").mouseenter(OnMouseEnter);

    $("#starPanel").mouseleave(OnMouseLeave);

    // init stars
    SetNthStar(getNthStarNum()); 
    

}



function ShowLernweltDialog(baseUrl) {
    var options = SP.UI.$create_DialogOptions();
    options.height = 500;
    options.width = 800;
    options.url = baseUrl;
    SP.UI.ModalDialog.showModalDialog(options);
}