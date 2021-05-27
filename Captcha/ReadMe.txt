(logic)
1. choose a voice captcha and grab the .wav audio file
2. cast it to a .flac file using the 'sox.exe' utility
3. use google-speech API to convert the audio into pure text 

(steps)
1. SOX תוכנה לעיבוד אודיו
   http://sox.sourceforge.net/

2. install google speech via nuget
   https://www.nuget.org/packages/Google.Cloud.Speech.V1/
   PM > Install-Package Google.Cloud.Speech.V1 -Version 2.1.0     

   more sources:
   https://cloud.google.com/speech-to-text
   https://cloud.google.com/speech-to-text/docs/quickstart-protocol
   https://github.com/googleapis/google-cloud-dotnet       

3. enable google speech api 
   Developer Console > find API > Enable 

4. create service account credentials 
   Developer Console > Generate JSON credentials > (Service account > Create > Keys > Add Key > create new key > (type) JSON > file will be downloaded)   

5. set credentials (see code samples)
   [option-1] Environment Variable - GOOGLE_APPLICATION_CREDENTIALS=/path/to/my/key.json 
   [option-2] use SpeechClientBuilder    

(code)
see 'Audio2Text - Sample Code.txt'