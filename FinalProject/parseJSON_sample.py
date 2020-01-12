import json

def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")

def parseBusinessData():
    #read the JSON file
    with open('../dbFiles/yelp_business.JSON','r') as f: 
        outfile =  open('business.txt', 'w')
        line = f.readline()
        while line:
            data = json.loads(line)
            outfile.write(cleanStr4SQL(data['business_id'])+',') #business id
            outfile.write(cleanStr4SQL(data['name'])+',') #name
            outfile.write(cleanStr4SQL(data['address'])+',') #full_address
            outfile.write(cleanStr4SQL(data['state'])+',') #state
            outfile.write(cleanStr4SQL(data['city'])+',') #city
            outfile.write(cleanStr4SQL(data['postal_code']) + ',')  #zipcode
            outfile.write(str(data['latitude'])+',') #latitude
            outfile.write(str(data['longitude'])+',') #longitude
            outfile.write(str(data['stars'])+',') #stars
            outfile.write(str(data['review_count'])+',') #reviewcount
            outfile.write(str(data['is_open'])+',') #openstatus
            outfile.write("\nCategories: ")
            outfile.write(str([item for item in  data['categories']])+'\t') #category list
            #outfile.write(str([])) # write your own code to process attribute
            outfile.write("\nHours: ")
            outfile.write(str({k:v for (k, v) in data['hours'].items()}))
            outfile.write('\n')

            line = f.readline()
    outfile.close()
    f.close()

def parseUserData():
    with open('../dbFiles/yelp_user.JSON','r') as f:  
        outfile =  open('user.txt', 'w')
        line = f.readline()
        while line:
            data = json.loads(line)

            outfile.write(str(data['user_id'])+',') #user_id
            outfile.write(str(data['name'])+',') #name
            outfile.write(str(data['average_stars'])+',') #average_stars
            outfile.write(str(data['cool'])+',') #cool count
            outfile.write(str(data['fans'])+',') #fans
            outfile.write(str(data['funny'])+',') #funny
            outfile.write(str(data['review_count'])+',') #review_count
            outfile.write(str(data['useful'])+',') #useful
            outfile.write(str(data['yelping_since'])+',') #yelping_since
            outfile.write("\nFriends: ")
            outfile.write(str([item for item in  data['friends']])) #friends
            outfile.write('\n')
            line = f.readline()
        outfile.close()
        f.close()

def parseCheckinData():
    with open('../dbFiles/yelp_checkin.JSON','r') as f:  
        outfile =  open('check_in.txt', 'w')
        line = f.readline()
        while line:
            data = json.loads(line)

            outfile.write(str(data['business_id'])+',') 
            outfile.write("\nTimes: ")
            outfile.write(str({k:v for (k,v) in data['time'].items()})) #time         
            outfile.write('\n')
            line = f.readline()
        outfile.close()
        f.close()


def parseReviewData():
    with open('../dbFiles/yelp_review.JSON','r') as f:  
        outfile =  open('review.txt', 'w')
        line = f.readline()
        while line:
            data = json.loads(line)

            outfile.write(str(data['review_id'])+',') #review_id
            outfile.write(str(data['user_id'])+',') #user_id
            outfile.write(str(data['business_id'])+',') #business_id
            outfile.write(str(data['stars'])+',') #stars
            outfile.write(str(data['date'])+',') #date
            outfile.write(str(data['useful'])+',') #useful
            outfile.write(str(data['funny'])+',') #funny
            outfile.write(str(data['cool'])+',') #cool
            outfile.write(str(data['text'])+',') #text
            outfile.write('\n')
            line = f.readline()
        outfile.close()
        f.close()

parseBusinessData()
parseUserData()
parseCheckinData()
parseReviewData()
