const LIGHT_ACTIVE_CLASS = 'active';
const ACTIVE_TIME = 500;
// const turnAllLightsOff = (lights) => {
//   lights.forEach((light) => {
//     light.classList.remove(LIGHT_ACTIVE_CLASS);
//   });
// }
function reveal() {
    var reveals = document.querySelectorAll(".show-on-scroll");
    //debugger;
    reveals.forEach((light, index) => {
      setTimeout(() => {
        light.classList.add(LIGHT_ACTIVE_CLASS);
        // if(index === lights.length - 1) {
        //   setTimeout(() => {
        //     turnAllLightsOff(lights);
        //   }, ACTIVE_TIME);
        // }
      }, index * ACTIVE_TIME);
    });
  }

//window.addEventListener("scroll", reveal);
//document.getElementById('platform').addEventListener("scroll", reveal);

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
