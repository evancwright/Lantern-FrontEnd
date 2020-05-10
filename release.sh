#!/bin/sh

#After running this script,
#copy the release directory to Lantern_Staging
#and rename it 'Lantern'.
#Tar zip and tar this folder
#Commit and push the zip and tar from there

rm -rf release
rm -rf ../Lantern_Release/Lantern
rm -rf ../Lantern_Release/Lantern.tar
mkdir release
mkdir release/bin
mkdir release/notes
mkdir release/examples
echo "copying assembly language files"
cp XTAC/XTAC/bin/Debug/*.dll release
cp Lantern/LASM/bin/Debug/LASM.dll release
cp Lantern/LangDef/bin/Debug/LangDef.dll release
cp Lantern/ZXBinToTape/bin/Debug/zxbin2tap.exe release/bin
cp XTAC/XTAC/bin/Debug/Lantern.exe release
cp XTAC/XTAC/bin/Debug/notes/*.* release/notes
cp Player/bin/Debug/Player.exe release
cp -r XTAC/XTAC/bin/Debug/bin release
cp -r XTAC/XTAC/bin/Debug/Merlin32_v1.0 release
cp -r XTAC/XTAC/bin/Debug/docs release
cp -r XTAC/XTAC/bin/Debug/6502skel release
cp -r XTAC/XTAC/bin/Debug/CoCoSkel release
cp -r XTAC/XTAC/bin/Debug/CoCoLib release
cp -r XTAC/XTAC/bin/Debug/z80skel release
cp -r XTAC/XTAC/bin/Debug/trs80 release
#cp -r XTAC/XTAC/bin/Debug/apple2 release
cp -r XTAC/XTAC/bin/Debug/spectrum release
cp -r XTAC/XTAC/bin/Debug/cpc464 release
cp -r XTAC/XTAC/bin/Debug/BBCMicro release
cp -r XTAC/XTAC/bin/Debug/8086Skel release
#cp -r XTAC/XTAC/bin/Debug/RPiSkel release
cp -r XTAC/XTAC/bin/Debug/CCommon release
cp -r XTAC/XTAC/bin/Debug/6502Merlin release
cp -r XTAC/XTAC/bin/Debug/c64Merlin release
cp -r XTAC/XTAC/bin/Debug/Apple2Merlin release
cp -r XTAC/XTAC/bin/Debug/LASMSkel release
cp -r XTAC/XTAC/bin/Debug/cpm release
echo "copying sample projects" 
cp XTAC/XTAC/bin/Debug/LondonAdventure.xml release/examples
cp XTAC/XTAC/bin/Debug/UndergroundFactory.xml release/examples
cp XTAC/XTAC/bin/Debug/LockedDoor.xml release/examples
cp XTAC/XTAC/bin/Debug/PrisonEscape.xml release/examples
cp XTAC/XTAC/bin/Debug/InstantDeath.xml release/examples
cp XTAC/XTAC/bin/Debug/Flashlight.xml release/examples
cp XTAC/XTAC/bin/Debug/SecretPassage.xml release/examples
cp XTAC/XTAC/bin/Debug/ColdAndHungry.xml release/examples
cp XTAC/XTAC/bin/Debug/MonsterDrop.xml release/examples
cp XTAC/XTAC/bin/Debug/CompoundObject.xml release/examples
cp XTAC/XTAC/bin/Debug/BlockingMonster.xml release/examples
cp XTAC/XTAC/bin/Debug/CombatDemo.xml release/examples
cp "XTAC/XTAC/bin/Debug/Heinlein_Station3.xml" release/examples
cp XTAC/XTAC/bin/Debug/RemoteLights.xml release/examples
cp XTAC/XTAC/bin/Debug/LadderDemo.xml release/examples
cp XTAC/XTAC/bin/Debug/NoLightDeath.xml release/examples
cp XTAC/XTAC/bin/Debug/BoatExample.xml release/examples
cp XTAC/XTAC/bin/Debug/BreakableWallDemo.xml release/examples
cp XTAC/XTAC/bin/Debug/DigDemo.xml release/examples
cp XTAC/XTAC/bin/Debug/BridgeExample.xml release/examples

echo "Removing utils I don't have the right to distribute"
rm -f ../Lantern_Release/Lantern/bin/c1451.exe
rm -f ../Lantern_Release/Lantern/bin/x64.exe
#rm -f ../Lantern_Release/Lantern/bin/z80asm.exe
rm -f ../Lantern_Release/Lantern/bin/CPCDiskXP.exe
rm -f ../Lantern_Release/Lantern/bin/CPCTapeXP.exe


##echo "moving zip file to ../staging"
#mv release.zip ../Lantern_Staging
echo "copying release folder to ../Lantern_Release/"
cp -r release ../Lantern_Release/ 
echo "renaming Lantern_Release/release to Lantern_Release/Lantern"
mv ../Lantern_Release/release ../Lantern_Release/Lantern
tar -C  ../Lantern_Release -cvf ../Lantern_Release/Lantern.tar Lantern