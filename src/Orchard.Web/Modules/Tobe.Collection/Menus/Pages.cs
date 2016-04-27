using Orchard;
using Orchard.UI.Navigation;

namespace Tobe.Collection.Menus {
    public class AdminMenuPages : Component, INavigationProvider {
        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(T("Pages"), "5", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu) {
            menu.Add(T("List"), "1.0", item =>
                item.Action("List", "Admin", new { area = "Contents", id = "Page" }));

            menu.Add(T("New Page"), "1.1", item =>
                 item.Action("Create", "Admin", new { area = "Contents", id = "Page" }));
        }
    }
}