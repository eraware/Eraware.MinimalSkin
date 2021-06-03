export default class Menu {
    button: HTMLButtonElement;
    isOpen: boolean;
    ul: HTMLUListElement;
        
    /**
     * Initializes the menu system.
     */
    constructor(ul: HTMLUListElement, button: HTMLButtonElement) {
        this.button = button;
        this.isOpen = false;
        this.ul = ul;

        this.button.addEventListener('click', e => {
            e.preventDefault();
            this.toggle();
        });

        this.ul.addEventListener("expanded", e => {
            if (e.target !== this.ul){
                const oldHeight = parseInt(this.ul.style.height,10);
                const change = (e as CustomEvent).detail.change;
                this.ul.style.height = oldHeight + change + "px";
            }
        });

        this.ul.addEventListener("collapsed", e => {
            if (e.target !== this.ul){
                const oldHeight = parseInt(this.ul.style.height,10);
                const change = (e as CustomEvent).detail.change;
                this.ul.style.height = oldHeight - change + "px";
            }
        });

        this.attachButtons();
    }

    toggle() {
        if (this.isOpen){
            this.button.parentElement?.classList.remove("expanded");
            this.ul.style.height = "0";
            const collapsed = new CustomEvent(
                "collapsed",
                {
                    bubbles: true,
                    detail: {
                        change: this.ul.scrollHeight,
                    },
                });
            this.ul.dispatchEvent(collapsed);
        }
        else{
            this.button.parentElement?.classList.add("expanded");
            this.ul.style.height = this.ul.scrollHeight.toString() + "px";
            var parentUl = this.ul.closest("ul");
            if (parentUl){
                parentUl.style.height = parentUl.scrollHeight + "px";
            }
            const expanded = new CustomEvent(
                "expanded",
                {
                    bubbles: true,
                    detail: {
                        change: this.ul.scrollHeight,
                    },
                });
            this.ul.dispatchEvent(expanded);
        }

        this.isOpen = !this.isOpen;
    }

    attachButtons() {
        const children = this.ul.children;
        for (let index = 0; index < children.length; index++) {
            const li = children[index];
            if (li.classList.contains("hasChildren")){
                const ul = li.querySelector("ul") as HTMLUListElement;
                const button = li.querySelector("button") as HTMLButtonElement;
                new Menu(ul, button);
            }
        }
    }
}