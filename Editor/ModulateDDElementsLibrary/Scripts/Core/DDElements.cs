
namespace DandyDino.Modulate
{
    public static class DDElements
    {
        public static ReflectionUtilities ReflectionUtilities => _reflectionUtilities ??= new ReflectionUtilities();
        private static ReflectionUtilities _reflectionUtilities;
        
        public static Rendering Rendering => _rendering ??= new Rendering();
        private static Rendering _rendering;
        
        public static Lists Lists => _lists ??= new Lists();
        private static Lists _lists;
        
        public static EditorUtils EditorUtils => _editorUtils ??= new EditorUtils();
        private static EditorUtils _editorUtils;

        public static Layout Layout => _layout ??= new Layout();
        private static Layout _layout;

        public static Helpers Helpers => _helpers ??= new Helpers();
        private static Helpers _helpers;
        
        public static Listeners Listeners => _listeners ??= new Listeners();
        private static Listeners _listeners;

        public static Assets Assets => _assets ??= new Assets();
        private static Assets _assets;

        public static Icons Icons => _icons ??= new Icons();
        private static Icons _icons;
        
        public static Colors Colors => _colors ??= new Colors();
        private static Colors _colors;
        
        public static Styles Styles => _styles ??= new Styles();
        private static Styles _styles;
        
        public static Templates Templates => _templates ??= new Templates();
        private static Templates _templates;
    }
}