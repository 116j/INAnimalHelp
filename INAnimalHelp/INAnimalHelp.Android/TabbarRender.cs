using Android.Content;
using Android.Support.Design.Widget;
using Android.Views;
using INAnimalHelp;
using INAnimalHelp.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(AppShell), typeof(TabbarRender))]
namespace INAnimalHelp.Droid
{
    [Obsolete]
    internal class TabbarRender : TabbedPageRenderer, BottomNavigationView.IOnNavigationItemReselectedListener
    {

        private bool _isShiftModeSet;

        public TabbarRender(Context context) : base(context)
        {
        }
        /// <summary>
        /// Возвращение на корневую страницу при двойном нажатии на кнопку нижнего меню
        /// </summary>
        /// <param name="item"></param>
        public void OnNavigationItemReselected(IMenuItem item)
        {
            if (Element is AppShell)
            {
                AppShell mainTabPage = Element as AppShell;
                mainTabPage.CurrentPage.Navigation.PopToRootAsync();
            }
        }

        /// <summary>
        /// Установка функции возвращения на корневую страницу 
        /// при двойном нажатии на кнопку нижнего меню
        /// </summary>
        /// <param name="changed"></param>
        /// <param name="l"></param>
        /// <param name="t"></param>
        /// <param name="r"></param>
        /// <param name="b"></param>
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            try
            {
                base.OnLayout(changed, l, t, r, b);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            try
            {
                if (!_isShiftModeSet)
                {
                    List<global::Android.Views.View> children = GetAllChildViews(ViewGroup);

                    if (children.SingleOrDefault(x => x is BottomNavigationView) is BottomNavigationView bottomNav)
                    {
                        bottomNav.SetOnNavigationItemReselectedListener(this);
                        bottomNav.SetShiftMode(false, false);
                        _isShiftModeSet = true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error setting ShiftMode: {e}");
            }
        }
        /// <summary>
        /// Получение всех
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private List<global::Android.Views.View> GetAllChildViews(global::Android.Views.View view)
        {
            if (!(view is ViewGroup group))
            {
                return new List<global::Android.Views.View> { view };
            }

            List<global::Android.Views.View> result = new List<global::Android.Views.View>();

            for (int i = 0; i < group.ChildCount; i++)
            {
                global::Android.Views.View child = group.GetChildAt(i);

                List<global::Android.Views.View> childList = new List<global::Android.Views.View> { child };
                childList.AddRange(GetAllChildViews(child));

                result.AddRange(childList);
            }

            return result.Distinct().ToList();
        }
    }
}