import Menu from './menu/menu';
// import { migrateFa4ToFa5 } from './utilities/utils'; uncomment to migrate fa4 to fa5

document.addEventListener('DOMContentLoaded', () => {
    // migrateFa4ToFa5(); uncomment to migrate fa4 to fa5

    let header = document.querySelector("#dnnTheme>header");
    let nav = header?.querySelector("nav") as HTMLElement;
    let ul = nav.querySelector("ul") as HTMLUListElement;
    let navButton = nav?.querySelector("button") as HTMLButtonElement;
    new Menu(ul, navButton);
});