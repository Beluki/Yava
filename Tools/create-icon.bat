@echo off

REM PATH to the Imagemagick convert.exe program:
set imconvert=C:\Imagemagick\convert.exe


%imconvert% -density 500 -background none gnome-joystick.svg -resize 16x16 icon16px.png
%imconvert% -density 500 -background none gnome-joystick.svg -resize 24x24 icon24px.png
%imconvert% -density 500 -background none gnome-joystick.svg -resize 32x32 icon32px.png
%imconvert% -density 500 -background none gnome-joystick.svg -resize 48x48 icon48px.png
%imconvert% -density 500 -background none gnome-joystick.svg -resize 64x64 icon64px.png
%imconvert% -density 500 -background none gnome-joystick.svg -resize 128x128 icon128px.png
%imconvert% -density 500 -background none gnome-joystick.svg -resize 256x256 icon256px.png

%imconvert% icon16px.png  ^
            icon24px.png  ^
            icon32px.png  ^
            icon48px.png  ^
            icon64px.png  ^
            icon128px.png ^
            icon256px.png ^
            gnome-joystick.ico

del *.png

