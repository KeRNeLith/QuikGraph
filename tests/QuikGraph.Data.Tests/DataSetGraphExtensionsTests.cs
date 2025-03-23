using System;
using System.Data;
using NUnit.Framework;
using static QuikGraph.Data.Tests.GraphTestHelpers;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Data.Tests
{
    /// <summary>
    /// Tests related to <see cref="DataSetGraphExtensions"/>.
    /// </summary>
    [TestFixture]
    internal sealed class DataSetGraphExtensionsTests
    {
        [Test]
        public void ToGraph()
        {
            // Empty data set
            var dataSet = new DataSet();
            DataSetGraph graph = dataSet.ToGraph();

            AssertEmptyGraph(graph);
            Assert.AreSame(dataSet, graph.DataSet);

            // Only tables data set
            dataSet = new DataSet();

            var ships = new DataTable("Ships");
            var shipIdCol = new DataColumn("ShipID", typeof(int)) { Unique = true };
            ships.Columns.Add(shipIdCol);
            dataSet.Tables.Add(ships);

            var modules = new DataTable("Modules");
            var moduleIdCol = new DataColumn("ModuleID", typeof(int)) { Unique = true };
            modules.Columns.Add(moduleIdCol);
            dataSet.Tables.Add(modules);

            graph = dataSet.ToGraph();

            graph.AssertHasVertices(ships, modules);
            graph.AssertNoEdge();
            Assert.AreSame(dataSet, graph.DataSet);


            // Table with relations
            dataSet = new DataSet();

            var computers = new DataTable("Computers");
            var computerIdCol = new DataColumn("ComputerID", typeof(int)) { Unique = true };
            var computerBrandCol = new DataColumn("Brand", typeof(string));
            var computerWinVerCol = new DataColumn("WindowsVersion", typeof(string));
            computers.Columns.Add(computerIdCol);
            computers.Columns.Add(computerBrandCol);
            computers.Columns.Add(computerWinVerCol);
            dataSet.Tables.Add(computers);

            var users = new DataTable("Users");
            var userIdCol = new DataColumn("UserID", typeof(int)) { Unique = true };
            var userNameCol = new DataColumn("Name", typeof(string));
            users.Columns.Add(userIdCol);
            users.Columns.Add(userNameCol);
            dataSet.Tables.Add(users);

            var printers = new DataTable("Printers");
            var printerIdCol = new DataColumn("PrinterID", typeof(int)) { Unique = true };
            var printerBrandCol = new DataColumn("Brand", typeof(string));
            printers.Columns.Add(printerIdCol);
            printers.Columns.Add(printerBrandCol);
            dataSet.Tables.Add(printers);

            var phones = new DataTable("Phones");
            var phoneIdCol = new DataColumn("PhoneID", typeof(int)) { Unique = true };
            var phoneMacAddrCol = new DataColumn("MacAddress", typeof(string)) { Unique = true };
            var phoneNumberCol = new DataColumn("Number", typeof(string));
            phones.Columns.Add(phoneIdCol);
            phones.Columns.Add(phoneMacAddrCol);
            phones.Columns.Add(phoneNumberCol);
            dataSet.Tables.Add(phones);


            var use = new DataRelation("Use", userIdCol, computerIdCol);
            dataSet.Relations.Add(use);
            var connectedTo = new DataRelation("ConnectedTo", computerIdCol, printerIdCol);
            dataSet.Relations.Add(connectedTo);
            var phoneWith = new DataRelation("phoneWith", userIdCol, phoneIdCol);
            dataSet.Relations.Add(phoneWith);


            graph = dataSet.ToGraph();

            graph.AssertHasVertices(computers, users, printers, phones);
            AssertHasRelations(
                graph,
                new[]
                {
                    new DataRelationEdge(use),
                    new DataRelationEdge(connectedTo),
                    new DataRelationEdge(phoneWith)
                });
            Assert.AreSame(dataSet, graph.DataSet);
        }

        [Test]
        public void ToGraph_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => DataSetGraphExtensions.ToGraph(null));
        }

        [Test]
        public void ToGraphviz()
        {
            // Empty graph
            DataSetGraph graph = new DataSet().ToGraph();
            string expectedDot =
                @"digraph G {" + Environment.NewLine
                + @"node [shape=record, style=solid];" + Environment.NewLine
                + @"}";

            Assert.AreEqual(expectedDot, graph.ToGraphviz());

            var dataSet = new DataSet();

            var devices = new DataTable("Devices");
            var deviceId = new DataColumn("deviceId", typeof(int)) { Unique = true };
            var deviceVersionId = new DataColumn("versionId", typeof(int));
            var deviceName = new DataColumn("name", typeof(string));
            devices.Columns.Add(deviceId);
            devices.Columns.Add(deviceVersionId);
            devices.Columns.Add(deviceName);
            dataSet.Tables.Add(devices);

            var tasks = new DataTable("Tasks");
            var taskDeviceId = new DataColumn("deviceId", typeof(int));
            var taskActionId = new DataColumn("actionId", typeof(int));
            var taskContent = new DataColumn("task", typeof(string));
            tasks.Columns.Add(taskDeviceId);
            tasks.Columns.Add(taskActionId);
            tasks.Columns.Add(taskContent);
            dataSet.Tables.Add(tasks);

            var actions = new DataTable("Actions");
            var actionId = new DataColumn("actionId", typeof(int)) { Unique = true };
            var actionName = new DataColumn("action", typeof(string));
            actions.Columns.Add(actionId);
            actions.Columns.Add(actionName);
            dataSet.Tables.Add(actions);

            var versions = new DataTable("Versions");
            var versionId = new DataColumn("versionId", typeof(int)) { Unique = true };
            var version = new DataColumn("name", typeof(string));
            versions.Columns.Add(versionId);
            versions.Columns.Add(version);
            dataSet.Tables.Add(versions);

            dataSet.Relations.Add(new DataRelation("DevicesToTasks", deviceId, taskDeviceId));
            dataSet.Relations.Add(new DataRelation("ActionsToTasks", actionId, taskActionId));
            dataSet.Relations.Add(new DataRelation("VersionsToDevices", versionId, deviceVersionId));

            expectedDot =
                @"digraph G {" + Environment.NewLine
                + @"node [shape=record, style=solid];" + Environment.NewLine
                + @"0 [shape=record, label=""Devices | +\ deviceId\ :\ Int32\ unique\n+\ versionId\ :\ Int32\n+\ name\ :\ String""];" + Environment.NewLine
                + @"1 [shape=record, label=""Tasks | +\ deviceId\ :\ Int32\n+\ actionId\ :\ Int32\n+\ task\ :\ String""];" + Environment.NewLine
                + @"2 [shape=record, label=""Actions | +\ actionId\ :\ Int32\ unique\n+\ action\ :\ String""];" + Environment.NewLine
                + @"3 [shape=record, label=""Versions | +\ versionId\ :\ Int32\ unique\n+\ name\ :\ String""];" + Environment.NewLine
                + @"0 -> 1 [label=""DevicesToTasks""];" + Environment.NewLine
                + @"2 -> 1 [label=""ActionsToTasks""];" + Environment.NewLine
                + @"3 -> 0 [label=""VersionsToDevices""];" + Environment.NewLine
                + @"}";

            graph = dataSet.ToGraph();
            Assert.AreEqual(expectedDot, graph.ToGraphviz());
        }

        [Test]
        public void ToGraphviz_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => DataSetGraphExtensions.ToGraphviz(null));
        }
    }
}