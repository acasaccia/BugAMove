using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame2.menu
{

    public interface IMenuAction
    {
        event Action actionPerformed;
       // event Action actionPerformed;
    }
    public interface MenuComponentComposite : IMenuAction{ 
        void addChildComponent(MenuComponentComposite c);
        void removeChildComponent(MenuComponentComposite c);
        MenuComponentComposite removeChildComponent(int index);
        MenuComponentComposite getChild(int index);
        MenuComponentComposite getFather();
        IList<MenuComponentComposite> getAllChildren();
        void setFather(MenuComponentComposite father);
        void setBounds(Rectangle b);
        void setExpandedBounds(Rectangle b);
        Rectangle getBounds();
        void validate();

        void setExpanded(bool val);
        bool isExpanded();
        void setFocus(bool val);
        bool hasFocus();
        void setFont(SpriteFont font);
        string getName();
        void paintComponent(SpriteBatch spriteBatch);

        void OnActionPeformed();
        
    }
    public abstract class AbstractComponent : MenuComponentComposite
    {
        protected MenuComponentComposite father;
        protected Texture2D texture;
        protected Rectangle bounds;
        protected Rectangle expandedBounds;
        protected bool expanded, focus;
        protected SpriteFont font;
        protected string title;
        public event Action actionPerformed;
        protected Color focusColor;
        protected Color color;
        public void OnActionPeformed() 
        {
  ///////

            if (actionPerformed != null)
                this.actionPerformed();
        }
        
        public void setFather(MenuComponentComposite father)
        {
            this.father = father;
        }
        public string getName() {
            return this.title;
        }
        public MenuComponentComposite getFather()
        {
            return this.father;
        }
        public bool isExpanded(){
            return this.expanded;
        }
        public bool hasFocus() {
            return this.focus;
        }
        public void setBounds(Rectangle r)
        {
            this.bounds = r;
            this.validate();
        }
        public void setExpandedBounds(Rectangle r)
        {
            this.expandedBounds = r;
            this.validate();
        }
        public Rectangle getBounds()
        {
            return this.bounds;
        }
        public void setExpanded(bool val)
        {
            this.expanded = val;

        }
        public void setFocus(bool val)
        {
            this.focus = val;
        }
        public void setFont(SpriteFont font)
        {
            this.font = font;
        }

        public abstract MenuComponentComposite getChild(int index);
        public abstract void removeChildComponent(MenuComponentComposite c);
        public abstract MenuComponentComposite removeChildComponent(int index);
        public abstract void addChildComponent(MenuComponentComposite c);
        public abstract IList<MenuComponentComposite> getAllChildren();
        public abstract void validate();
        public abstract void paintComponent(SpriteBatch spriteBatch);
         
    }
    public abstract class MenuComponentLeaf : AbstractComponent
    {
       
        override public void addChildComponent(MenuComponentComposite c)
        {
            throw new NoChildException();
        }

        override public void removeChildComponent(MenuComponentComposite c)
        {
            throw new NoChildException();
        }

        override public MenuComponentComposite removeChildComponent(int index)
        {
            throw new NoChildException();
        }

        override public MenuComponentComposite getChild(int index)
        {
            throw new NoChildException();
        }

        override public  IList<MenuComponentComposite> getAllChildren() {
            throw new NoChildException();
        }
       
    }
    public abstract class MenuComponentContainer : AbstractComponent {

        protected IList<MenuComponentComposite> children;
       // protected  MenuComponentComposite father;
       
        public MenuComponentContainer() {
            this.children = new List<MenuComponentComposite>();
            this.father = null;
        }
        override public void addChildComponent(MenuComponentComposite c)
        {
            c.setFather(this);
            this.children.Add(c);
            
        }

        override public void removeChildComponent(MenuComponentComposite c)
        {
            this.children.Remove(c);
            c.setFather(null);
        }

        override public MenuComponentComposite removeChildComponent(int index)
        {
            MenuComponentComposite c = this.getChild(index);
            if (c != null)
            {
                this.removeChildComponent(c);
                c.setFather(null);
               
            }
            return c;
        }

        override public MenuComponentComposite getChild(int index)
        {
            if (this.children.Count > index)
                return this.children[index];
            return null;
        }
        override public  IList<MenuComponentComposite> getAllChildren()
        {
            return this.children;
        }
       
    }
    public class MenuEntryChoice : MenuComponentLeaf {
        
       
       // private SpriteFont font;
        

        public MenuEntryChoice(string title, Texture2D texture, SpriteFont font, Color color, Color focusCol) {
            this.title = title;
            this.texture = texture;
            this.font = font;
            this.color = color;
            this.focusColor = focusCol;
        }

        new public void setFocus(bool state)
        {
            if (this.expanded)
            {
                MenuComponentComposite c = this.getChild(0);
                if (c != null && state == true)
                    c.setFocus(true);
            }
            else {
                base.setFocus(state);
            }
        }
        public override void validate()
        {
            

        }
        public override void paintComponent(SpriteBatch spriteBatch) 
        {

            //Nor the component nor its father will be printed..so
            if (!this.getFather().isExpanded() && !this.expanded)
                return;


            if (this.texture != null)
            {
                spriteBatch.Draw(this.texture, this.bounds, Color.White);
            }

            if (!this.expanded)
            {
                Color c;
                if (this.focus)
                {
                    //  Console.WriteLine("has focus setting color " + this.focusColor);
                    c = this.focusColor;
                }
                else
                {
                    c = this.color;
                }
                Vector2 titleSize = this.font.MeasureString(this.title);
                float x = this.bounds.X + (this.bounds.Width / 2.0f) - (titleSize.X / 2);
                //put the title in the middle of the component
                spriteBatch.DrawString(this.font, this.title, new Vector2(x, this.bounds.Y),
                            c, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
            
            //    spriteBatch.DrawString(this.font, this.title, new Vector2(this.bounds.X, this.bounds.Y),
              //          c, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
            }
            else {
                if (title != null && this.expanded)
                {
                    Vector2 titleDim = this.font.MeasureString(title);
                    int leftCornerX = (int)(bounds.X + (bounds.Width / 2 - (titleDim.X / 2)));
                    // Rectangle r = new Rectangle(leftCorner, bounds.Y, (int)titleDim.X, (int)titleDim.Y);
                    //Console.WriteLine("expanded list: title " + this.title);
                    spriteBatch.DrawString(this.font, this.title, new Vector2(leftCornerX, this.bounds.Y), this.color, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
                }
            }
        }
    }
    public class VerticalListMenuContainer : MenuComponentContainer{


     //   private String title;
     //   private Texture2D texture;
        //private SpriteFont font;
        private int firstChildDeltaY = 0;// = this.bounds.Height * 20 / 100 ;
        private int leftMargin;
        private int childHeight, childWidth;
      //  private Color color, focusColor;
        public VerticalListMenuContainer(String title, Texture2D texture, SpriteFont font, Color color, Color focusCol) {
            this.title = title;
            this.texture = texture;
            this.font = font;
            this.color = color;
            this.focusColor = focusCol;
            this.expanded = false;
           // this.validate();
            
        }
        public override void validate()
        {
            //this.childHeight = (this.bounds.Height * 10) / 100;
            //this.childWidth = (this.bounds.Width * 10) / 100;
            //this.leftMargin = this.bounds.X + ((this.bounds.Width * 10) / 100);
            //firstChildDeltaY = this.bounds.Y + (( this.bounds.Height * 20) / 100 );

            this.childHeight = (this.expandedBounds.Height * 10) / 100;
            this.childWidth = this.expandedBounds.Width;//(this.expandedBounds.Width * 10) / 100;
            this.leftMargin = 0;// this.expandedBounds.X + ((this.expandedBounds.Width * 10) / 100);
            firstChildDeltaY = this.expandedBounds.Y + ((this.expandedBounds.Height * 20) / 100);
            for (int i = 0; i < this.children.Count; i++) {
                children[i].setBounds(new Rectangle(this.leftMargin, this.firstChildDeltaY + this.expandedBounds.Y + (this.childHeight * i),
                    this.childWidth, this.childHeight));
                children[i].setExpandedBounds(this.expandedBounds);
            }

        }
        override public void addChildComponent(MenuComponentComposite c) {
         //   Console.WriteLine("list add child component " + this.children.Count);

            Rectangle b = new Rectangle(this.leftMargin, this.firstChildDeltaY + this.bounds.Y + (this.childHeight * this.children.Count),
                this.bounds.Width - this.leftMargin, this.childHeight);
            c.setBounds(b);
            c.setExpandedBounds(this.expandedBounds);
            base.addChildComponent(c);

        }
        public override void paintComponent(SpriteBatch spriteBatch)
        {
           // Console.WriteLine(this.title + " expanded " + this.expanded + " focus " + this.focus);
            if (this.father == null)
                return;
            
            if (this.texture != null && this.expanded)
            {
               
            }
            //must have an expanded father or should be expanded itself
            if (!expanded && this.getFather().isExpanded())
            {
                Color c;
                if (this.focus == true)
                {
                    c = focusColor;
                }
                else {
                    c = color;
                }
                Vector2 titleSize = this.font.MeasureString(this.title);
                float x = this.bounds.X + (this.bounds.Width / 2.0f) - (titleSize.X / 2);
                spriteBatch.DrawString(this.font, this.title, new Vector2(x, this.bounds.Y), c, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
                
           //     spriteBatch.DrawString(this.font, this.title, new Vector2(this.bounds.X, this.bounds.Y), c, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
                
                return;
            }
            

            if (title != null && this.expanded) {
                spriteBatch.Draw(this.texture, this.expandedBounds, Color.White);
                //Vector2 titleDim = this.font.MeasureString(title);
                //int leftCornerX = (int)( bounds.X + (bounds.Width / 2 - (titleDim.X / 2))) ;
             
             //   spriteBatch.DrawString(this.font, this.title, new Vector2(leftCornerX, this.bounds.Y), this.color, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
            }
            foreach (MenuComponentComposite item in this.children)
            {
                item.paintComponent(spriteBatch);
            }

         
            
        }


    }
    public class RootMenuItem : MenuComponentContainer
    {
        public int height;
        public int width;
        //public Texture2D texture;
        private int childDeltaX = 0;//5;
        private int childDeltaY = 0;//5; //percentage
        
        public RootMenuItem(int height, int width, Texture2D bgTexture) {
            this.height = height;
            this.width = width;
            this.texture = bgTexture;

            this.bounds = new Rectangle(0, 0, width, height);
        }
        public override void validate()
        {
        
            
        }
        override public void addChildComponent(MenuComponentComposite c) {

          //  Console.WriteLine("Root addChildComponent ");
            if (this.children.Count > 0)
                return; // max one child

            int x = (this.bounds.Width * this.childDeltaX) / 100;
            int y = (this.bounds.Height * this.childDeltaY) / 100;


            Rectangle childBounds = new Rectangle(this.bounds.X + x, this.bounds.Y + y, this.bounds.Width - x *2, this.bounds.Height - y *2);
            c.setBounds(childBounds);
            c.setExpandedBounds(this.bounds);
            base.addChildComponent(c);
        }

        public override void paintComponent(SpriteBatch spriteBatch)
        {

            if (this.texture != null)
            {
                //Console.WriteLine("RootMenuItem: paint " + width + " h " + height );
                spriteBatch.Draw(this.texture, this.bounds, Color.White);
                //spriteBatch.DrawString(this.font, "hello world", new Vector2(0, 0), Color.Red, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
                
            }
            foreach (var item in this.children)
            {
                item.paintComponent(spriteBatch);
            }
        }
        
    }
}
