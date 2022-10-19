const LIGHT_ACTIVE_CLASS = 'active';
const ACTIVE_TIME = 500;
function reveal() {
    var reveals = document.querySelectorAll(".show-on-scroll");
  
    reveals.forEach((light, index) => {
      setTimeout(() => {
        light.classList.add(LIGHT_ACTIVE_CLASS);
     
      }, index * ACTIVE_TIME);
    });
  }

window.addEventListener("scroll", ()=>{
  if(document.getElementById('platform')!=null)
  {
    if(document.documentElement.scrollTop>document.getElementById('platform').getBoundingClientRect().top+500){
      reveal();
    }
  }
});
function showMenu(){
  document.getElementById("menu-body").style.display = 'block';
  document.getElementById("body-content").style.display = 'none';
  document.querySelector(".menu-btn-container .close").style.display = 'block';
  document.querySelector(".menu-btn-container .menu").style.display = 'none';
}
function hideMenu(){
  document.getElementById("menu-body").style.display = 'none';
  document.getElementById("body-content").style.display = 'block';
  document.querySelector(".menu-btn-container .close").style.display = 'none';
  document.querySelector(".menu-btn-container .menu").style.display = 'block';
}

let fileInput = document.getElementById("inputTag");
let imageName = document.getElementById("imageName")

if(fileInput!=null)
{
fileInput.addEventListener("change", ()=>{
    let inputImage = document.querySelector("input[type=file]").files[0];

    imageName.innerText = inputImage.name;
})
}
