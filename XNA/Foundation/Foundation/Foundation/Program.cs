using System;

namespace Foundation
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Foundation game = new Foundation())
            {
                game.Run();
            }
        }
    }
#endif
}

