

from midiutil import MIDIFile
import json


degrees  = [60, 62, 64, 65, 67, 69, 71, 72]  # MIDI note number
track    = 0
channel  = 0
time     = 0    # 1 = 1/4
duration = 0.25    # 1 = 1/4
tempo    = 60   # In BPM
volume   = 100  # 0-127, as per the MIDI standard

midiFile = MIDIFile() 

midiFile.addTempo(track, 0, 60) #track, time, tempo

# for i, pitch in enumerate(degrees):
#     midiFile.addNote(track, channel, pitch, time + i/4, duration, volume)

# with open("C:\\Users\\yves\\Google Drive\\AIT\\X. Other\\Frusciantifier\\Frusciantifier\\major-scale.mid", "wb") as output_file:
#     midiFile.writeFile(output_file)

with open("C:\\Users\\yves\\Google Drive\\AIT\\X. Other\\Frusciantifier\\Frusciantifier\\Song.json", "r") as Song:
	notes = json.load(Song)
	for note in notes:
		midiFile.addNote(track, channel, note['Degree'], note['Time'], note['Duration'], volume)

with open("C:\\Users\\yves\\Google Drive\\AIT\\X. Other\\Frusciantifier\\Frusciantifier\\finalmidi.mid", "wb") as output_file:
	midiFile.writeFile(output_file)
