using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using WindowsGame2.menu;
using PuzzleBobbleInputHandling;

namespace WindowsGame2
{
  public class Game1 : Microsoft.Xna.Framework.Game
  {
    public GraphicsDeviceManager graphics;

    public static Game1 instance = null;
    public static Game1 getInstance() {
        return instance;
    }
    public Game1()
    {
      graphics = new GraphicsDeviceManager(this);
     // graphics.PreferredBackBufferHeight = 600;
     // graphics.PreferredBackBufferWidth = 300;


      graphics.PreferredBackBufferHeight = 768;
      graphics.PreferredBackBufferWidth = 1024;
//#if !DEBUG
 //     graphics.IsFullScreen = true;
      graphics.PreferMultiSampling = true;
//#endif
      graphics.ApplyChanges();
      Content.RootDirectory = "Content";

      
    }

    protected override void Initialize()
    {
        instance = this;
        Components.Add(new GamerServicesComponent(this));
        //Components.Add(new)
        Components.Add(new GameLogic(this));

        Rendering renderer = new Rendering(this);

        Menu menu = new Menu(this);
        this.Components.Add(menu);
        Components.Add(renderer);
        Components.Add(new MenuInputController(this));
      //this.Services.AddService(typeof (Menu),menu);
        InputManager inputManager = new InputManager(this);
        inputManager.pause();
        Components.Add(inputManager);

        base.Initialize();
    }

    //protected override void Update(GameTime gameTime)
    //{
    //  if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
    //    this.Exit();

    //  base.Update(gameTime);
    //}

    ///// <summary>
    ///// This is called when the game should draw itself.
    ///// </summary>
    ///// <param name="gameTime">Provides a snapshot of timing values.</param>
    //protected override void Draw(GameTime gameTime)
    //{
    //  base.Draw(gameTime);
    //}
  }
}
