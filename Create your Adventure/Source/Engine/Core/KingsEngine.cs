using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Window;

namespace Create_your_Adventure.Source.Engine.Core
{
    public static class KingsEngine
    {
        // ══════════════════════════════════════════════════
        // START
        // ══════════════════════════════════════════════════
        public static void Start()
        {
            Logger.Info("[ENGINE] ═══════════════════════════════════════════");
            Logger.Info("[ENGINE] Kings Engine Starting...");
            Logger.Info("[ENGINE] ═══════════════════════════════════════════");

            // ═══════════════════════════════════════════════════════════
            // PHASE 1: WINDOW MANAGER INITIALIZATION
            // ═══════════════════════════════════════════════════════════
            var windowManager = WindowManager.Instance;
            windowManager.Initialize(new WindowSettings
            {
                Title = "Create your Adventure",
                Width = 1920,
                Height = 1080
            });

            Logger.Info("[ENGINE] WindowManager initialized");

            // ═══════════════════════════════════════════════════════════
            // PHASE 2: GAMELOOP CREATION
            // (GameLoop constructor wires up all Silk.NET events)
            // (Other managers are initialized in GameLoop.OnLoad)
            // ═══════════════════════════════════════════════════════════
            var gameLoop = new GameLoop(windowManager);

            Logger.Info("[ENGINE] GameLoop created and wired");

            // ═══════════════════════════════════════════════════════════
            // PHASE 3: RUN MAIN LOOP
            // (Blocks until window closes)
            // ═══════════════════════════════════════════════════════════
            Logger.Info("[ENGINE] Entering main loop...");
            try
            {
                windowManager.Run();
            }

            catch (Exception ex)
            {
                Logger.Error($"[ENGINE] Fatal crash: {ex}");
                throw;
            }

            finally
            {
                windowManager.Dispose();
                Logger.Info("[ENGINE] Cleanup complete");
            }

            // ═══════════════════════════════════════════════════════════
            // PHASE 4: CLEANUP (After window closes)
            // (GameLoop.OnClose handles manager disposal in LIFO order)
            // ═══════════════════════════════════════════════════════════

            Logger.Info("[ENGINE] ═══════════════════════════════════════════");
            Logger.Info("[ENGINE] Kings Engine Shutdown Complete ✓");
            Logger.Info("[ENGINE] ═══════════════════════════════════════════");
        }
    }
}