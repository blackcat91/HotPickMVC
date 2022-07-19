$(document).ready(() => {
    var bool = false;
    $("#submit").css("opacity", 0)
    $("#edit").click((e) => {
        e.preventDefault();

        $(".border input").prop("disabled", bool);
        bool = !bool;
        if (!bool) {
            $("#submit").css("display", "none")
        }
        else {
            $("#submit").prop("disabled", false) 
            $("#submit").css("opacity", 1)
            $("#submit").css("display", "inline-block")
               
        }
    });

    
    
})