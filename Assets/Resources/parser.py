import re
import sys
file = open(sys.argv[1],'r')
data = file.readlines()

newfile = open('parsed_' + sys.argv[1],'w')

noAtoms = data[3].split(' ')[1]
noBonds = data[3].split(' ')[2]
print(noAtoms,noBonds)

newfile.write(noAtoms+'\n')
newfile.write(noBonds+'\n')

s = ''
for d in data[4:4+ int(noAtoms) + int(noBonds)]:
    s = re.sub('\s+',' ',d).strip().replace(' ','|')
    newfile.write(s+'\n')


newfile.write('teknas')