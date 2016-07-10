AIMLAudit
==================

AIML response analysis tool developed by [Kino Coursey](https://github.com/Kinoc) for [HansonRobotics](https://github.com/hansonrobotics/).

Contact: kino.coursey@gmail.com

---
## General
This reads tab seperated files containing user input and frequency to a chatbot and returns an analysis of responses and rule usage.

It uses the C# AIMLBotSharp as base.
It reuses C# v4.0 and was generated using Visual Studio 2010.

## Relevant files and directories 
Executable	.\AIMLAudit\bin\Debug\AIMLAudit.exe 
Configuration .\AIMLAudit\bin\Debug\Config\Settings.xml
User Long File .\AIMLAudit\bin\Debug\usercount.txt
AIML Directory .\AIMLAudit\bin\Debug\aimlos

## Operation
Modify the configuration file, Run the executable, then wait for the AIML under analysis to load.
You may use the appropriate browse button to change the user logs or the output directory (or you can type them in manually).
"usercount.txt" is from the ChatScript corpus,
Press "ThreadedSurvey" button. This will use batches of multiple CPU threads to analyze of the user input log.
Progress should be reported in the bottm list box.

The process can take one to multiple hours to process usercount.txt, hence the curIncrementalThr.txt and progress feedback.

## Notes
Files with *_p.txt instead of reporting counts  report Math.Log((double)frequency_count / (double)totals)

The system tries to sample to do a form of balanced testing. It uses the following formula to determine how many times it tests a given input.
```
	double log_n = Math.Log10(n);
	int reps = 2 + (int)log_n;
```
It then apportions N/reps "user input counts" to each trial.
Most of this was to get fair run time, and meaningful counts for debugging.

All tests are treated as single trials. That is the bot is reset as if the input tested was it's first input. Thus <THAT> plays little to no role in processing.

In the file field, some entries do not point to files but to processing states.

* AANULL = no valid response found.
* MAXRECRUSION = possible infinite loop

##Input files
*usercount.txt:
The Chatscript logs accumulated and sorted into a TSV format.

_user-input_ \t _count_

##Output files

* totalCountThr.txt:
The total number of user inputs. Can be used to compute probability.

* curResponsesThr.txt:
System responses accumulated and sorted in a TSV format.

_user-input_ \t _system-response_ \t _count_

* patternMatchCount.txt and patternMatchCount_p.txt:

 _source file_ \t _pattern_ \t _count_

   Activation counts sorted by file.

* patternMatchCountByVal.txt and patternMatchCountByVal_p.txt:

_source file_ \t _pattern_ \t _count_

  Activation counts sorted by frequency. This includes BOTH final responses and SRAI's used to get to them.

* patternMatchRespondsCountByVal.txt and patternMatchRespondsCountByVal_p.txt:

_source file_ \t _pattern_ \t _count_

  Activation counts sorted by file. This includes ONLY counts for the category that produced a response.

* normalizedMatchCount.txt:

  _user-input_ \t _final-file-used_ \t _final-path-used_ \t _system-response_ \t _count_

  Category tracing + responses. The normalized version lower cases and strips punctuation from the user input and merges the responses and traces into a common list. Merges responses that appear to be different textually but are the same to the matching process.

So you probably want to look at curResponsesThr.rar and patternMatchRespondsCount.txt to get a good idea about what she says and why.
You optinally may want to sort NormalizedMatchCount.txt. That way you could see problematic responses and know what path in which file produced it and use the count to see how important it was to fix (versus just collect statistics).

Happy Bot building!
