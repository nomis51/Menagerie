import os
from tensorflow.keras import Sequential
from tensorflow.keras.layers import Conv2D, MaxPool2D, Dense, Flatten
from tensorflow.keras.optimizers import RMSprop

os.environ['CUDA_VISIBLE_DEVICES'] = '-1'
training_name = "currency_type"
image_size = (46, 46)
model_file_path = "./trained/{}/{}.h5".format(training_name, training_name)
classes_file_path = "./trained/{}/{}-classes.txt".format(
    training_name, training_name)


def load_model(num_classes):
    if os.path.exists(model_file_path):
        model = Sequential([
            Conv2D(16, (3, 3), activation="relu", input_shape=(image_size[0], image_size[1], 3)),
            MaxPool2D(2, 2),

            Conv2D(32, (3, 3), activation="relu"),
            MaxPool2D(2, 2),

            Conv2D(64, (3, 3), activation="relu"),
            MaxPool2D(2, 2),

            Flatten(),

            Dense(512, activation="relu"),
            Dense(num_classes, activation="softmax")
        ])

        model.compile(
            loss="categorical_crossentropy",
            optimizer=RMSprop(learning_rate=0.001),
            metrics=["accuracy"]
        )

        model.load_weights(model_file_path)

        return model
    else:
        return None, None
