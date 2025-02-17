﻿using System.Reflection;
using System.ComponentModel.Design;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Shell;
using PeterWibeck.ScrumyVSPlugin;

namespace ScrumyVSPlugin_UnitTests.MenuItemTests
{
    [TestClass()]
    public class MenuItemTest
    {
        /// <summary>
        /// Verify that a new menu command object gets added to the OleMenuCommandService. 
        /// This action takes place In the Initialize method of the Package object
        /// </summary>
        [TestMethod]
        public void InitializeMenuCommand()
        {
            // Create the package
            IVsPackage package = new ScrumyVSPluginPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

            //Verify that the menu command can be found
            CommandID menuCommandID = new CommandID(PeterWibeck.ScrumyVSPlugin.GuidList.guidScrumyVSPluginCmdSet, (int)PeterWibeck.ScrumyVSPlugin.PkgCmdIDList.cmdScrumySettings);
            System.Reflection.MethodInfo info = typeof(Package).GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(info);
            OleMenuCommandService mcs = info.Invoke(package, new object[] { (typeof(IMenuCommandService)) }) as OleMenuCommandService;
            Assert.IsNotNull(mcs.FindCommand(menuCommandID));
        }

        [TestMethod]
        public void MenuItemCallback()
        {
            // Create the package
            IVsPackage package = new ScrumyVSPluginPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Create a UIShell service mock and proffer the service so that it can called from the MenuItemCallback method
            BaseMock uishellMock = UIShellServiceMock.GetUiShellInstance();
            serviceProvider.AddService(typeof(SVsUIShell), uishellMock, true);

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

            //Invoke private method on package class and observe that the method does not throw
            System.Reflection.MethodInfo info = package.GetType().GetMethod("MenuItemCallback", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(info, "Failed to get the private method MenuItemCallback throug refplection");
            info.Invoke(package, new object[] { null, null });

            //Clean up services
            serviceProvider.RemoveService(typeof(SVsUIShell));

        }
    }
}
