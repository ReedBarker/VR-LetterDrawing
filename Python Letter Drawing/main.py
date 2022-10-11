import sklearn
import pandas as pd
import numpy as np
from sklearn import linear_model
import pickle


data = pd.read_csv("*\\Letter Drawing\\Unity VR Letter Drawing\\Database.data")
predictValue = "Value"

unityOutput = open("*\\Letter Drawing\\Unity VR Letter Drawing\\DATA1.data")

stuff = unityOutput.readlines()

unityOutputContents = []
for x in stuff:
    unityOutputContents.append(float(x))

print(unityOutputContents)

X = np.array(data.drop([predictValue], 1))  # returns a new dataframe array without predict
y = np.array(data[predictValue])  # returns dataframe array with only predict

'''# Remove ' to build the model
best = 0
loopCount = 1000
for _ in range(loopCount):
    x_train, x_test, y_train, y_test = sklearn.model_selection.train_test_split(X, y, test_size=0.1)
    linear = linear_model.LinearRegression()  # liner is set to a linear regression model
    linear.fit(x_train, y_train)  # fits the x_train data and y_train data to a line in the model
    acc = linear.score(x_test, y_test)  # linear.score represents the accuracy
    if acc > best:
        best = acc
        print(_, acc)
        with open("*\\Letter Drawing\\Unity VR Letter Drawing\\coordinatesModel.pickle", "wb") as f:
            pickle.dump(linear, f)'''


pickle_in = open("*\\Letter Drawing\\Unity VR Letter Drawing\\\\coordinatesModel.pickle", "rb")
linear = pickle.load(pickle_in)

new_input = [unityOutputContents]
new_output = linear.predict(new_input)

# print(new_output)

outputletter = ''
if new_output > 1000:
    outputletter = 'C'
elif new_output > 100:
    outputletter = 'S'
else:
    outputletter = 'O'

print(outputletter)

letterFile = open("*\\Letter Drawing\\Unity VR Letter Drawing\\letter.data", "w")
letterFile.write(outputletter)
