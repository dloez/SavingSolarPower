using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {
        //temporary blocks
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

        //list of all solar panels to get stats
        List<IMySolarPanel> panels = new List<IMySolarPanel>();
        //list of all batteries to get stats
        List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
        //lcd to show stats
        IMyTextSurface lcd;

        public Program() {
            //fill batteries list
            GridTerminalSystem.GetBlockGroupWithName("SavingSolarPower").GetBlocks(blocks);
            foreach(IMyTerminalBlock block in blocks) {
                batteries.Add(block as IMyBatteryBlock);
            }

            lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName("SavingSolarPower");

            //initial configuration of lcd panel
            lcd.ContentType = ContentType.TEXT_AND_IMAGE;
            lcd.FontSize = 1;
            lcd.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
            lcd.WriteText("");

            //solar panels to get stats
            GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(panels);

        }

        public void Main(string argument, UpdateType updateSource) {
            int maxStoredPower = getMaxStoredPower();
            int currentStoredPower = getCurrentStoredPower();

            int allCurrentOutput = getAllCurrentOutput();
            int avgCurrentOutput = getAvgOutput(allCurrentOutput);
            writeOutputInName();

            lcd.WriteText("SavingSolarPower\n" +
                "-------------------------------------------------------------\n" +
                "Current Output Average\n" +
                getPorcentageString(avgCurrentOutput, 140) + avgCurrentOutput.ToString() + " kW\n" +
                "Total Current Output:\n" +
                allCurrentOutput.ToString() + " kW || " + allCurrentOutput / 1000 + " MW\n" + 
                "-------------------------------------------------------------\n" + 
                "Max Stored Power\n" +
                maxStoredPower.ToString() + " MW/h\n" +
                "Current Stored Power\n" +
                getPorcentageString(currentStoredPower, maxStoredPower) + currentStoredPower + " MW/h");
        }

        public int getAllCurrentOutput() {
            int allCurrentOutput = 0;

            foreach (IMySolarPanel panel in panels) {
                allCurrentOutput += (int)(panel.CurrentOutput * 1000);
            }

            return allCurrentOutput;
        }

        public int getAvgOutput(int allCurrentOutput) {
            return allCurrentOutput / panels.Count;
        }

        public string getPorcentageString(int avgCurrentOutput, int rule) {
            StringBuilder sb = new StringBuilder("[");

            int numberOfLines = 0;
            if(avgCurrentOutput != 0) {
                numberOfLines = avgCurrentOutput * 10 / rule;
            }

            for(int i = 0; i < 10; i++) {
                if (i < numberOfLines) {
                    sb.Append("#");
                } else {
                    sb.Append("%");
                }
            }

            sb.Append("] " + numberOfLines * 10 + "% || ");

            return sb.ToString();
        }

        public void writeOutputInName() {
            foreach(IMySolarPanel panel in panels) {
                panel.CustomName = "Solar Panel " + (int) (panel.CurrentOutput * 1000) + " kW";
            }
        }

        public int getMaxStoredPower() {
            int maxStoredPower = 0;

            foreach(IMyBatteryBlock battery in batteries) {
                maxStoredPower +=(int) battery.MaxStoredPower;
            }

            return maxStoredPower;
        }

        public int getCurrentStoredPower() {
            int currentStoredPower = 0;

            foreach (IMyBatteryBlock battery in batteries) {
                currentStoredPower += (int)battery.CurrentStoredPower;
            }

            return currentStoredPower;
        }
}
