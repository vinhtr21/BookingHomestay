let x = document.cookie;
if(x.includes("UserId"))
{
    document.getElementById("btnLogin").style.display = "none";
    document.getElementById("userProfile").style.display = "block";
}else{
    document.getElementById("btnLogin").style.display = "block";
    document.getElementById("userProfile").style.display = "none";
}
console.log($("#btnLogin"));

let profile = document.querySelector('.profile');
let menu = document.querySelector('.menu');

profile.onclick = function () {
    menu.classList.toggle('active');
}