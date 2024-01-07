# HUMap
This is the source code for a simple map and timetable app for the university of hull, the app shows users where a timetabled event is on the university map.
You can find the compiled app here: https://play.google.com/store/apps/details?id=com.cormacdev.map

This app won't compile from the repository as the keystore is not included, the google API for the map is also linked to the keystore and will not work. To compile a working app you will need to remove the keystore requirement in the csproj file and change the api in the android manifest file.
