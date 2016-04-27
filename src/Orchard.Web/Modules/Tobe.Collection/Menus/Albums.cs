using Orchard;
using Orchard.UI.Navigation;

namespace Tobe.Collection.Menus {
    public class AdminMenuAlbums : Component, INavigationProvider {
        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(T("Albums"), "5", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu) {
            menu.Add(T("List"), "1.0", item =>
                item.Action("List", "Admin", new { area = "Contents", id = "Album" }));

            menu.Add(T("New Album"), "1.1", item =>
                 item.Action("Create", "Admin", new { area = "Contents", id = "Album" }));
        }
    }
}