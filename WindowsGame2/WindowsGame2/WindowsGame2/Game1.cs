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


      
#if !DEBUG
      graphics.IsFullScreen = true;
#else
      graphics.PreferredBackBufferHeight = 768;
      graphics.PreferredBackBufferWidth = 1024;
#endif


      graphics.PreferMultiSampling = true;
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

  }
}
