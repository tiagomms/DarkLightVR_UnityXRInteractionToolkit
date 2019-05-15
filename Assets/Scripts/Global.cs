using System;
/*
 * alternative way of sharing state (global) between game objects
 */

public static class Global
{
    [Flags]
    public enum ThisLevelNbr
    {
        L1 = 0, L2A = 1, L2B = 2, L3 = 3, L4 = 4, L5 = 5, L6 = 6, L2A_EASTER = 7, L2B_EASTER = 8, L3_EASTER = 9, L4_EASTER = 10, L5_EASTER = 11
    }

    public static ThisLevelNbr currentLevel;

    public static class Shared_Events {
        static public string LOAD_SCENE = "LOAD_SCENE";
        static public string CHANGE_SCENE = "CHANGE_SCENE";
        static public string IN_MEDITATION_CIRCLE = "IN_MEDITATION_CIRCLE";
        static public string OUT_MEDITATION_CIRCLE = "OUT_MEDITATION_CIRCLE";

        // CONTROLLERS
        static public string SET_TELEPORT = "SET_TELEPORT";
        static public string SET_VOICECOMMAND = "SET_VOICECOMMAND";
        static public string SET_SELECTION_RAY = "SET_SELECTION_RAY";
        static public string SET_HINTMENU = "SET_HINTMENU";
        static public string GO_AWAY_INPUT = "GO_AWAY_INPUT";

        static public string TURN_ON_VOICE_INPUT = "TURN ON";
        static public string TURN_OFF_VOICE_INPUT = "TURN OFF";
        static public string SUCCESSFUL_VOICE_INPUT = "SUCCESSFUL";

        public static string NARRATOR_EVENT_BETWEEN_CLIPS = "NARRATOR_EVENT_BETWEEN_CLIPS";
        public static string NARRATOR_ENDED = "NARRATOR_ENDED";
        public static string SHOW_MEDITATION_CIRCLE = "SHOW_MEDITATION_CIRCLE";
        static public string SET_DONTDESTROY_OBJECTS = "SET_DONTDESTROY_OBJECTS";

    }

    public static class Shared_Controllers {
        static public bool TELEPORT = true;
        static public bool VOICECOMMAND = true;
        static public bool SELECTION_RAY = true;
        static public float SELECTION_RAY_MAX_DISTANCE = 10f;
        static public bool GRAB = true;
        static public bool HINTMENU = true;
        static public bool MEDITATION_CIRCLE_READY = false;

        static public bool ENDED_GAME = false;
        public static bool FOUND_EASTER_EGG = false;
    }

    public enum ConsciousnessLevel {
        FULLY = 0,
        BECOMING = 1,
        NOT = 2
    }

    public static ConsciousnessLevel ConsciousLevel = ConsciousnessLevel.FULLY;
    public static string GetConsciousnessLevelString(ConsciousnessLevel cLevel) {
        return Enum.GetName(typeof(ConsciousnessLevel), cLevel);
    }

    public static class Level1_Details {
        static public int TUTORIALS_COMPLETED = 0;
    }

    public static string GetSharedHintString(Shared_Hints tut)
    {
        return Enum.GetName(typeof(Shared_Hints), tut);
    }    
    public enum Shared_Hints {
        NONE = 0,
        TUT_TELEPORT = 1,
        TUT_HINTMENU = 2,
        TUT_SELECTIONRAY = 3,
        TUT_CANCELSELECTION = 4,
        TUT_GRAB = 5,
        TUT_THROW = 6,
        TUT_GOAWAY = 7,
        TUT_IAMREADY = 8
    }
    public static class Level2_Events {
        // level events
        static public string RESET_BALL = "RESET_BALL";
        static public string THROW_BALL = "THROW";
        static public string TURN_AROUND = "TURN_AROUND";
        public static string START_TURN_AROUND = "START_TURN_AROUND";
        public static string TRASH_PILLING_UP = "TRASH_PILLING_UP";

        static public int score = 0;

    }
    public static class Level3_Events {

    }
    public static class Level4_Events {
        public static string TRASH_FALLING_START = "TRASH_FALLING_START";
        public static string PLAYER_HIT_TRASH = "PLAYER_HIT_TRASH";
        public static string STONE_APPEAR = "STONE_APPEAR";
        public static string AFTER_MIRACLE_OCCURED = "AFTER_MIRACLE_OCCURED";

        public static string LIGHT_SPIRITS_STOP = "LIGHT_SPIRITS_STOP";
        public static string LIGHT_SPIRITS_RAISE_ARM = "LIGHT_SPIRITS_RAISE_ARM";
        public static string MOVE_SPAWN_LOCATIONS = "MOVE_SPAWN_LOCATIONS";
    }
    public static class Level5_Events {

    }


}
