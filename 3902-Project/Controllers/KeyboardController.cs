﻿using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Project.Controllers;

public abstract class KeyboardController : IController
{
    public abstract void Update();

    // This controller exists to share this static variable between all keyboard controllers for smooth operation when transitioning between them.
    public static HashSet<Keys> PreviousPressedKeys { get; set; }
}
