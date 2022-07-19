$(document).ready(() => {

    
    var addBtn = $(".addBtn");
    createOnClick(".add", ".addStock", ".removeStock")
    createOnClick(".remove", ".removeStock", ".addStock")


   
 
});



var createOnClick = (class1, class2, class3) => {
    $(class1).click(function () {
        if ($(class3).attr("class").includes("active")) {
            $(class3).removeClass("active")
        }
        $(class2).toggleClass("active")

    });



}