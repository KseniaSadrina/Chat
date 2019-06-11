#!/usr/bin/env python3
# Copyright 2018-present, HKUST-KnowComp.
# All rights reserved.
#
# This source code is licensed under the license found in the
# LICENSE file in the root directory of this source tree.
"""A script to run the reader model interactively."""

import sys
sys.path.append('.')
import torch
import argparse
import logging
import prettytable
import time
import os
from tqdm import tqdm
import json

from predictor import Predictor
from multiprocessing import cpu_count

PREDICTOR = None

def main(args):
    examples = []
    qids = []
    with open(args.data_file,encoding='utf-8') as f:
        data = json.load(f)
        for pair in data:
            contexts = pair['contexts']
            question = pair['question']
            qaid = pair['id']
            qids.append(qaid)
            examples.append((contexts, question))

    results = {}
    for i in range(0, len(examples)):
      saveable_res = []
      for context in examples[i][0]: # run the test for each given context, in the end choose the one with the best score
        split_context = context.split('.')
        for sentence in split_context:
          if (sentence == '' or sentence == None):
            continue
          res = PREDICTOR.predict(sentence, examples[i][1], None, 1)
          for mini_res in res:
            # save the answer only if it has the highest match
            pred_out = {}
            pred_out['answer'] = str(mini_res[0])
            pred_out['score'] = float(mini_res[1])
            saveable_res.append(pred_out)
      results[qids[i]] = saveable_res
    with open(args.output_file, "w") as writer:
        writer.write(json.dumps(results, indent=4) + "\n")

# ------------------------------------------------------------------------------
# Commandline arguments & init
# ------------------------------------------------------------------------------

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--model', type=str, default=None,
                        help='Path to model to use')
    parser.add_argument('--data-file', type=str, default=None,
                        help='Path to place where the question and the context can be found')
    parser.add_argument('--output-file', type=str, default=None,
                        help='Path to the results location')
    parser.add_argument('--embedding-file', type=str, default=None,
                        help=('Expand dictionary to use all pretrained '
                            'embeddings in this file.'))
    parser.add_argument('--char-embedding-file', type=str, default=None,
                        help=('Expand dictionary to use all pretrained '
                            'char embeddings in this file.'))
    parser.add_argument('--num-workers', type=int, default=int(cpu_count()/2),
                        help='Number of CPU processes (for tokenizing, etc)')
    parser.add_argument('--no-cuda', action='store_true',
                        help='Use CPU only')
    parser.add_argument('--gpu', type=int, default=-1,
                        help='Specify GPU device id to use')
    parser.add_argument('--no-normalize', action='store_true',
                        help='Do not softmax normalize output scores.')
    args = parser.parse_args()

    args.cuda = not args.no_cuda and torch.cuda.is_available()
    if args.cuda:
        torch.cuda.set_device(args.gpu)

    PREDICTOR = Predictor(
        args.model,
        normalize=not args.no_normalize,
        embedding_file=args.embedding_file,
        char_embedding_file=args.char_embedding_file,
        num_workers=args.num_workers,
    )
    if args.cuda:
        PREDICTOR.cuda()
    main(args)

