import numpy as np
import random
import copy
import pickle
import math
import sys
import struct

#random.seed(1)

def floatToBits(f):
    s = struct.pack('>f', f)
    return struct.unpack('>l', s)[0]


def bitsToFloat(b):
    s = struct.pack('>l', b)
    return struct.unpack('>f', s)[0]


def f(x):
    assert(x <= 5)
    assert(x >= 0)
    return math.sinh(math.cos(x)*math.cos(x)+1)


def gen_population(size):
    p = []
    for i in range(0, size):
        p.append(random.uniform(0, 5))

    return p


def eval_population(p):
    """ Cheie = f(x)
        Valoare = x
    """
    fitness = list(map(f, p))
    return dict(zip(fitness, p))


def roulette(p, n):
    fitness_to_population = copy.deepcopy(p)
    selected = []
    for i in range(0, 2):
        normalized_fitness = list(map(lambda x: 1/x, fitness_to_population.keys()))
        sum_fitness = sum(normalized_fitness)
        probabilities = list(map(lambda p: p/sum_fitness, normalized_fitness))

        #print(probabilities)
        #print(sum(probabilities))

        r = random.random()
        pos = 0
        s = probabilities[pos]

        while s < r and pos < len(probabilities):
            s += probabilities[pos]
            pos += 1

        #print("pos=", pos)
        #print("s=", s)
        #print("r=", r)

        if pos == len(probabilities):
            pos -= 1

        fitness = list(fitness_to_population.keys())[pos]
        selected.append(fitness_to_population[fitness])
        del fitness_to_population[fitness]

    return selected


def normalize(x):
    if x > 5:
        return 5

    if x < 0:
        return 0

    if math.isnan(x):
        x = random.uniform(0,5)

    return x


def mutation(x, n):
    for i in range(0, n):
        bits = floatToBits(x)
        #print(bin(bits))

        r = random.randint(0, 30)
        mask = 1 << r
        #print(bin(mask))

        #print(bin(mask^bits))
        x = bitsToFloat(mask^bits)
        #print(x)

    x = normalize(x)

    return x


def crossover(x1, x2):
    bits1 = floatToBits(x1)
    bits2 = floatToBits(x2)
    #print(bin(bits1))
    #print(bin(bits2))

    r = random.randint(0, 31)
    #print("r=", r)
    mask = 1
    for i in range (0,r):
        mask = mask << 1
        mask+=1

    #print(bin(mask))

    tail1 = bits1&mask
    tail2 = bits2&mask

    bits1 = bits1^tail1 | tail2
    bits2 = bits2^tail2 | tail1

    x1 = bitsToFloat(bits1)
    x2 = bitsToFloat(bits2)

    x1 = normalize(x1)
    x2 = normalize(x2)

    return x1,x2


if '__main__' == __name__:
    population_size = 100
    p = gen_population(population_size)
    #f = open("p.pkl", "wb")
    #pickle.dump(p, f)
    #f.close()
    epsilon = 10 ** -15
    num_iter = 100

    #p = pickle.load(open("p.pkl", "rb"))

    #print(p)
    fitness_to_population = eval_population(p)
    #print(fitness_to_population)

    #x = {0.5:1, 0.2:2, 0.3:4}
    #print(x)
    #print(roulette(x, 2))
    #print(x)
    #exit(1)

    #print(bitsToFloat(floatToBits(3.14)))
    #exit(1)

    #mutation(3.14, 2)
    #exit(1)

    #crossover(3.14, 1.7)
    #exit(1)

    minimum = sys.float_info.max
    new_minimum = min(fitness_to_population.keys())

    print(new_minimum)

    t=0
    while abs(minimum - new_minimum) > epsilon or t < num_iter:
        best_fit = {}

        # move the best half of the first population
        sorted_fitness = sorted(fitness_to_population.keys())
        #print(sorted_fitness)
        for i in range(0, population_size//2):
            #print(sorted_fitness[i], fitness_to_population[sorted_fitness[i]])
            best_fit[sorted_fitness[i]] = fitness_to_population[sorted_fitness[i]]

        #print(len(best_fit))
        #print("roulette")

        new_p = copy.deepcopy(list(best_fit.values()))
        #print(new_p)

        i=0
        while i < population_size//2:
            num_chromosomes = random.randint(1, 2)
            selected_chromosomes = roulette(best_fit, num_chromosomes)

            if num_chromosomes == 1:
                #print('mutation')
                new_p.append(mutation(selected_chromosomes[0], 2))
                i+=1
            else:
                #print('crossover')
                x1, x2 = crossover(selected_chromosomes[0], selected_chromosomes[1])
                new_p.append(x1)
                new_p.append(x2)
                i+=2


        #print(new_p)
        #print("size " + str(len(new_p)))
        fitness_to_population = eval_population(new_p)
        #print(fitness_to_population)

        minimum = new_minimum
        new_minimum = min(fitness_to_population.keys())
        print(new_minimum)
        #print(abs(minimum - new_minimum))
        t+=1

    sorted_fitness = sorted(fitness_to_population.keys())
    print("f({})={}".format(fitness_to_population[sorted_fitness[0]], sorted_fitness[0]))
