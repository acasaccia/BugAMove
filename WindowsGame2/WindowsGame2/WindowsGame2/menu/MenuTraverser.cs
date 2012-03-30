using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame2.Sound;
using Microsoft.Xna.Framework;

namespace WindowsGame2.menu
{
    public class MenuTraverser
    {
        RootMenuItem menu;
        MenuComponentComposite currentSelectedComponent;
        Stack<int> componentsTraversedIndexes;

      //  public Action<MenuTraverser.Actions> OnMenuActionCachedHandler; 
        public enum Actions { MOVE_FORWARD, MOVE_BACKWARD, MOVE_UP, MOVE_DOWN, ACTION_PERFORMED, MOUSE_MOVED, MOUSE_CLICKED, KINECT_HOVERING }
        int currentComponentIndex;

        public float hoverTime = 0.0f;
        public float hoverTimeThreshold = 5.0f;

        

        public MenuTraverser(RootMenuItem menu)
        {
            this.menu = menu;
        //    this.OnMenuActionCachedHandler = (action) => this.OnMenuAction(action);

            this.componentsTraversedIndexes = new Stack<int>();
            MenuComponentComposite firstChild = this.menu.getChild(0);


            if (firstChild != null)
            {
                firstChild.setExpanded(true);
               
                this.currentSelectedComponent = firstChild;
                this.currentComponentIndex = 0;

                MenuComponentComposite c = firstChild.getChild(0);
                //first child of the first child of root is a container with at least one child. Start traversing from here
                if (c != null)
                {
                    this.componentsTraversedIndexes.Push(this.currentComponentIndex);
                    this.currentSelectedComponent = c;
                    this.currentSelectedComponent.setFocus(true);
                    this.currentComponentIndex = 0;
                    // this.currentSelectedComponent.setExpanded(true);

                }
                else {
                    firstChild.setFocus(true);
                }

            }
            else { 
                //that's pretty strange. a root component should always have a child component
            }
       
          
        }

        public void doAction() { 
        
        }
        public void back()
        {
            MenuComponentComposite f = this.currentSelectedComponent.getFather();

            if (f == this.menu.getChild(0))
                return;
            try
            {
                if (!this.currentSelectedComponent.isExpanded() && this.currentSelectedComponent.hasFocus())
                {
                    

                    
                    this.currentSelectedComponent.setFocus(false);
                    f.getFather().setExpanded(true);
                    f.getFather().validate();
                    f.setExpanded(false);
                    this.currentSelectedComponent = f;
                    this.currentComponentIndex = this.componentsTraversedIndexes.Pop();
                    f.setFocus(true);
                  //  this.currentSelectedComponent.setFocus(true);
                }
                else {
                    //should be a container expanded without child
                 //   Console.WriteLine("Closing container expanded without child");
                    this.currentSelectedComponent.setFocus(true);
                    this.currentSelectedComponent.setExpanded(false);
                    f.setExpanded(true);
                    f.validate();
                }
                
            }
            catch (NoChildException e)
            {

            }
        }
        private void goForward() {

            MenuComponentComposite t = this.currentSelectedComponent;

            MenuComponentComposite c = this.currentSelectedComponent.getChild(0);

            MenuComponentComposite containerChild = this.currentSelectedComponent;
            containerChild.setExpanded(true);
            // containerChild.setBounds(this.currentSelectedComponent.getFather().getBounds());
            MenuComponentComposite father = this.currentSelectedComponent.getFather();
            father.setExpanded(false);

            if (c != null)
            {
                //current component is a container..can go forward
                //if (c != this.menu)
                this.currentSelectedComponent.setFocus(false);

                //  Console.WriteLine("container child " + containerChild.); 

                //close the previousopen container 




                this.componentsTraversedIndexes.Push(this.currentComponentIndex);
                this.currentSelectedComponent = c;
                this.currentComponentIndex = 0;
                this.currentSelectedComponent.setFocus(true);

            }
            else
            {
                //  this.currentSelectedComponent.setFocus(false);
            }
        
        }
        public void forward() {
         //   MenuComponentComposite t = this.currentSelectedComponent;
            try
            {
                goForward();
                
            }
            catch (NoChildException e)
            {
                if (Menu.soundEnabled)
                {
                    MenuSoundManager.playError();
                }
            }

        }
        public void up()
        {
            MenuComponentComposite f = this.currentSelectedComponent.getFather();

            if (f != null)
            {
                IList<MenuComponentComposite> sibling = f.getAllChildren();
                this.currentSelectedComponent.setFocus(false);
                this.currentComponentIndex = (this.currentComponentIndex + sibling.Count - 1) % sibling.Count;
                this.currentSelectedComponent = sibling[this.currentComponentIndex];
                this.currentSelectedComponent.setFocus(true);

                if (Menu.soundEnabled) {
                    MenuSoundManager.playMoveUp();
                }
            }
        }
        public void down()
        {
            MenuComponentComposite f = this.currentSelectedComponent.getFather();

            if (f != null)
            {
                this.currentSelectedComponent.setFocus(false);
                IList<MenuComponentComposite> sibling = f.getAllChildren();
                this.currentComponentIndex = (this.currentComponentIndex + 1) % (sibling.Count );
                this.currentSelectedComponent = sibling[this.currentComponentIndex];
                this.currentSelectedComponent.setFocus(true);

                if (Menu.soundEnabled)
                {
                    MenuSoundManager.playMoveDown();
                }
            }
        }
        public void onKinectAction(Actions action, Point coord, float dt)
        {
            var bounds = currentSelectedComponent.getBounds();
            
            switch (action)
            {
            
                case Actions.KINECT_HOVERING:
                    if (hoverTime > hoverTimeThreshold)
                    {
                        this.hoverTime = 0;
                        this.OnMenuAction(Actions.ACTION_PERFORMED);
                    }
                    if (coord.Y < bounds.Y && this.currentComponentIndex > 0)
                    {
                        this.hoverTime = 0;
                        this.up();
                    }
                    else if (coord.Y > bounds.Y + bounds.Height &&
                        this.currentComponentIndex < this.currentSelectedComponent.getFather().getChildNum() - 1)
                    {
                        this.hoverTime = 0;
                        this.down();
                    }
                    else {
                        var b = this.currentSelectedComponent.getBounds();
                        int x = (int)this.currentSelectedComponent.getFont().MeasureString(this.currentSelectedComponent.getName()).X;
                        if (coord.X > (b.Width / 2) - (x / 2) &&
                            coord.X < (b.Width / 2) + (x / 2))
                            this.hoverTime += dt;
                        else
                            this.hoverTime = 0;
                        
                        //TODO: measure the real length of the component name
                        float W = this.currentSelectedComponent.getFont().MeasureString(this.currentSelectedComponent.getName()).X;
                        //
                       
                    }
                    break;
                default:
                    break;
            }
        }
        public void onCursorAction(Actions action, Point coord)
        {
            //Console.WriteLine("MenuTraverser: onCursorAction");
            var bounds = currentSelectedComponent.getBounds();
            switch (action)
            {
            
                case Actions.MOUSE_MOVED:
                        var b = this.currentSelectedComponent.getBounds();
                        int x = (int)this.currentSelectedComponent.getFont().MeasureString(this.currentSelectedComponent.getName()).X;
                        if (coord.X > (b.Width / 2) - (x / 2) && coord.X < (b.Width / 2) + (x / 2)) {
            
                            if (coord.Y < bounds.Y && this.currentComponentIndex > 0){
                                this.up();
                            }
                            else if (coord.Y > bounds.Y + bounds.Height && 
                                this.currentComponentIndex < this.currentSelectedComponent.getFather().getChildNum() - 1) {
                                this.down();
                            }
          
                        }
                        break;
                case Actions.MOUSE_CLICKED:
                    break;
                
                default:
                    break;
            }
            
            
        }
        public void OnMenuAction(Actions action) {
          //  Console.WriteLine("MenuTraverser OnMenuAction");
            switch (action)
            {
                case Actions.MOVE_UP:
                    this.up();
                    break;
                case Actions.MOVE_DOWN:
                    this.down();
                    break;
                case Actions.MOVE_FORWARD:
                    this.forward();
                    break;
                case Actions.MOVE_BACKWARD:
                    this.back();
                    break;
                case Actions.ACTION_PERFORMED:
                    try
                    {
                        goForward();

                    }
                    catch (NoChildException e)
                    {
                      this.currentSelectedComponent.OnActionPeformed();
        
                    }
                    break;
                 
                
            }
        }
    }
}
